using MediatR;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Application.Features.Commands.VerifyPayment;

public class VerifyPaymentHandler(ITransactionRepository txRepository)
    : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResponseDto>
{
    public async Task<VerifyPaymentResponseDto> Handle(VerifyPaymentCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null)
            return new VerifyPaymentResponseDto{ IsSuccess = false, Message = "توکن نامعتبر است" };

        // Expiration check
        if (tx.Status == PaymentStatus.Pending && tx.CreatedAt.AddMinutes(2) < DateTime.UtcNow)
        {
            await txRepository.UpdateStatusAsync(tx, PaymentStatus.Expired, null, req.AppCode);
            return new VerifyPaymentResponseDto{ IsSuccess = false, Status = "Expired", Amount = tx.Amount, ReservationNumber = tx.ReservationNumber, Message = "زمان پرداخت منقضی شده است" };
        }

        tx.AppCode = req.AppCode;
        await txRepository.UpdateStatusAsync(tx, tx.Status, tx.RRN, req.AppCode);

        return tx.Status switch
        {
            PaymentStatus.Success => new VerifyPaymentResponseDto{ IsSuccess = true, Status = "Success", Amount = tx.Amount, Rrn = tx.RRN, ReservationNumber = tx.ReservationNumber, Message = "پرداخت با موفقیت تایید شد" },
            PaymentStatus.Failed => new VerifyPaymentResponseDto{ IsSuccess = false, Status = "Failed", Amount = tx.Amount, ReservationNumber = tx.ReservationNumber, Message = "پرداخت ناموفق بود" },
            _ => new VerifyPaymentResponseDto{ IsSuccess = false, Status = "Pending", Amount = tx.Amount, ReservationNumber = tx.ReservationNumber, Message = "پرداخت هنوز انجام نشده است" }
        };
    }
}