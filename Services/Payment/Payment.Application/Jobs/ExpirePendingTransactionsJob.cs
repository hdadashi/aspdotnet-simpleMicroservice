using Payment.Application.EventBus;
using Payment.Domain.Interfaces;
using Quartz;

namespace Payment.Application.Jobs;

public class ExpirePendingTransactionsJob(ITransactionRepository txRepository, IMessagePublisher publisher) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var threshold = TimeSpan.FromMinutes(2);
        await txRepository.ExpirePendingOlderThan(threshold);
    }
}