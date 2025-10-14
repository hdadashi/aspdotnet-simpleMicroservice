using Payment.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Contracts;
using Payment.Application.EventBus;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(ITransactionRepository txRepository, IMessagePublisher publisher, IConfiguration cfg)
    : ControllerBase
{
    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] GetTokenRequest req)
    {
        var token = Guid.NewGuid().ToString();
        var tx = new Transaction
        {
            TerminalNo = req.TerminalNo,
            Amount = req.Amount,
            RedirectUrl = req.RedirectUrl,
            ReservationNumber = req.ReservationNumber,
            PhoneNumber = req.PhoneNumber,
            Token = token,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await txRepository.CreateAsync(tx);

        var gatewayUrl = $"{cfg["GatewayBaseUrl"] ?? "https://localhost:5002"}/api/gateway/pay/{token}";

        return Ok(new GetTokenResponse
        {
            IsSuccess = true,
            GatewayUrl = gatewayUrl,
            Token = token,
            Message = "توکن با موفقیت ایجاد شد"
        });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest req)
    {
        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null) return BadRequest(new { isSuccess = false, message = "توکن نامعتبر است" });

        // expire check: older than 2 minutes => Expired
        if (tx.Status == PaymentStatus.Pending && tx.CreatedAt.AddMinutes(2) < DateTime.UtcNow)
        {
            await txRepository.UpdateStatusAsync(tx, PaymentStatus.Expired, rrn: null, appCode: req.AppCode);
            return Ok(new { isSuccess = false, status = "Expired", amount = tx.Amount, reservationNumber = tx.ReservationNumber, message = "زمان پرداخت منقضی شده است" });
        }

        // store appCode
        tx.AppCode = req.AppCode;
        await txRepository.UpdateStatusAsync(tx, tx.Status, rrn: tx.RRN, appCode: req.AppCode); // just update appcode

        return tx.Status switch
        {
            PaymentStatus.Success => Ok(new
            {
                isSuccess = true,
                status = "Success",
                amount = tx.Amount,
                rrn = tx.RRN,
                reservationNumber = tx.ReservationNumber,
                message = "پرداخت با موفقیت تایید شد"
            }),
            PaymentStatus.Failed => Ok(new
            {
                isSuccess = false,
                status = "Failed",
                amount = tx.Amount,
                reservationNumber = tx.ReservationNumber,
                message = "پرداخت ناموفق بود"
            }),
            _ => Ok(new
            {
                isSuccess = false,
                status = "Pending",
                amount = tx.Amount,
                reservationNumber = tx.ReservationNumber,
                message = "پرداخت هنوز انجام نشده است"
            })
        };
        // still pending
    }

    // Internal API for Gateway to call with result
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest req)
    {
        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null) return BadRequest(new { isSuccess = false, message = "توکن نامعتبر است" });

        var newStatus = req.IsSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
        await txRepository.UpdateStatusAsync(tx, newStatus, req.Rrn);

        // publish event
        var ev = new PaymentProcessedEvent {
            Token = tx.Token,
            ReservationNumber = tx.ReservationNumber,
            Amount = tx.Amount,
            Status = newStatus.ToString(),
            Rrn = tx.RRN,
            ProcessedAt = DateTime.UtcNow
        };
        publisher.PublishPaymentProcessed(ev);

        return Ok(new { isSuccess = true, message = "وضعیت با موفقیت بروزرسانی شد" });
    }
}