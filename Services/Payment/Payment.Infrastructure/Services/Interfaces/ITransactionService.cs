using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Application.Features.Interfaces;

public interface ITransactionService
{
    Task<Transaction> CreateAsync(Transaction tx);
    Task<Transaction?> GetByTokenAsync(string token);
    Task UpdateStatusAsync(Transaction tx, PaymentStatus status, string? rrn = null, string? appCode = null);
    Task ExpirePendingOlderThan(TimeSpan threshold);
}