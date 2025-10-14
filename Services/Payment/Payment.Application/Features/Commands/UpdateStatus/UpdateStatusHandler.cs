using MediatR;
using Payment.Application.EventBus;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Application.Features.Commands.UpdateStatus;

public class UpdateStatusHandler(
    ITransactionRepository txRepository,
    IMessagePublisher publisher)
    : IRequestHandler<UpdateStatusCommand, object>
{
    public async Task<object> Handle(UpdateStatusCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null)
            return new { isSuccess = false, message = "توکن نامعتبر است" };

        var newStatus = req.IsSuccess ? PaymentStatus.Success : PaymentStatus.Failed;

        await txRepository.UpdateStatusAsync(tx, newStatus, req.Rrn);

        var ev = new PaymentProcessedEvent
        {
            Token = tx.Token,
            ReservationNumber = tx.ReservationNumber,
            Amount = tx.Amount,
            Status = newStatus.ToString(),
            Rrn = tx.RRN,
            ProcessedAt = DateTime.UtcNow
        };

        publisher.PublishPaymentProcessed(ev);

        return new
        {
            isSuccess = true,
            message = "وضعیت با موفقیت بروزرسانی شد",
            status = newStatus.ToString(),
            rrn = tx.RRN,
            reservationNumber = tx.ReservationNumber,
            amount = tx.Amount
        };
    }
}