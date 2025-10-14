using Microsoft.EntityFrameworkCore;
using Payment.Application.Features.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Infrastructure.Persistence;

namespace Payment.Infrastructure.Services;

public class TransactionService(PaymentContext db) : ITransactionService
{
    public async Task<Transaction> CreateAsync(Transaction tx)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            db.Transactions.Add(tx);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
            return tx;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public Task<Transaction?> GetByTokenAsync(string token) =>
        db.Transactions.FirstOrDefaultAsync(t => t.Token == token);

    public async Task UpdateStatusAsync(Transaction tx, PaymentStatus status, string? rrn = null, string? appCode = null)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            tx.Status = status;
            if (rrn != null) tx.RRN = rrn;
            if (appCode != null) tx.AppCode = appCode;
            tx.UpdatedAt = DateTime.UtcNow;

            db.Transactions.Update(tx);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task ExpirePendingOlderThan(TimeSpan threshold)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var cutoff = DateTime.UtcNow - threshold;

            var items = await db.Transactions
                .Where(t => t.Status == PaymentStatus.Pending && t.CreatedAt <= cutoff)
                .ToListAsync();

            foreach (var t in items)
            {
                t.Status = PaymentStatus.Expired;
                t.UpdatedAt = DateTime.UtcNow;
            }

            if (items.Count != 0)
            {
                await db.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}