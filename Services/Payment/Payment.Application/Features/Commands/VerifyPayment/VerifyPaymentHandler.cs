using MediatR;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Application.Features.Commands.VerifyPayment;

public class VerifyPaymentHandler(ITransactionRepository txRepository)
    : IRequestHandler<VerifyPaymentCommand, object>
{
    public async Task<object> Handle(VerifyPaymentCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null)
            return new { isSuccess = false, message = "توکن نامعتبر است" };

        // Expiration check
        if (tx.Status == PaymentStatus.Pending && tx.CreatedAt.AddMinutes(2) < DateTime.UtcNow)
        {
            await txRepository.UpdateStatusAsync(tx, PaymentStatus.Expired, null, req.AppCode);
            return new { isSuccess = false, status = "Expired", amount = tx.Amount, reservationNumber = tx.ReservationNumber, message = "زمان پرداخت منقضی شده است" };
        }

        tx.AppCode = req.AppCode;
        await txRepository.UpdateStatusAsync(tx, tx.Status, tx.RRN, req.AppCode);

        return tx.Status switch
        {
            PaymentStatus.Success => new { isSuccess = true, status = "Success", amount = tx.Amount, rrn = tx.RRN, reservationNumber = tx.ReservationNumber, message = "پرداخت با موفقیت تایید شد" },
            PaymentStatus.Failed => new { isSuccess = false, status = "Failed", amount = tx.Amount, reservationNumber = tx.ReservationNumber, message = "پرداخت ناموفق بود" },
            _ => new { isSuccess = false, status = "Pending", amount = tx.Amount, reservationNumber = tx.ReservationNumber, message = "پرداخت هنوز انجام نشده است" }
        };
    }
}