using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction tx);
    Task<Transaction?> GetByTokenAsync(string token);
    Task UpdateStatusAsync(Transaction tx, PaymentStatus status, string? rrn = null, string? appCode = null);
    Task ExpirePendingOlderThan(TimeSpan threshold);
}