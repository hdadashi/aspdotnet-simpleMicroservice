using MediatR;
using Payment.Application.EventBus;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Application.Features.Commands.UpdateStatus;

public class UpdateStatusHandler(
    ITransactionRepository txRepository,
    IMessagePublisher publisher)
    : IRequestHandler<UpdateStatusCommand, UpdateStatusResponseDto>
{
    public async Task<UpdateStatusResponseDto> Handle(UpdateStatusCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var tx = await txRepository.GetByTokenAsync(req.Token);
        if (tx == null)
            return new UpdateStatusResponseDto{ IsSuccess = false, Message = "توکن نامعتبر است" };

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

        return new UpdateStatusResponseDto
        {
            IsSuccess = true,
            Message = "وضعیت با موفقیت بروزرسانی شد",
            Status = newStatus.ToString(),
            Rrn = tx.RRN,
            ReservationNumber = tx.ReservationNumber,
            Amount = tx.Amount
        };
    }
}