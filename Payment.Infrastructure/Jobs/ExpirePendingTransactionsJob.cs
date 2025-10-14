using Payment.Application.EventBus;
using Payment.Application.Features.Interfaces;
using Quartz;

namespace Payment.Infrastructure.Jobs;

public class ExpirePendingTransactionsJob(ITransactionService txService, IMessagePublisher publisher) : IJob
{
    private readonly IMessagePublisher _publisher = publisher;

    public async Task Execute(IJobExecutionContext context)
    {
        // expire pending older than 2 minutes
        var threshold = TimeSpan.FromMinutes(2);
        await txService.ExpirePendingOlderThan(threshold);
    }
}