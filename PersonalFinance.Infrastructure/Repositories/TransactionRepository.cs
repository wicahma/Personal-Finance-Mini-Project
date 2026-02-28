using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public TransactionRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Transactions
                       .Include(x => x.Account)
                       .Include(x => x.Category)
                       .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
                       .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Transactions
                       .Where(x => x.UserProfileId == userProfileId)
                       .Include(x => x.Account)
                       .Include(x => x.Category)
                       .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
                       .OrderByDescending(x => x.Date)
                       .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Transactions
                       .Where(x => x.AccountId == accountId)
                       .Include(x => x.Category)
                       .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
                       .OrderByDescending(x => x.Date)
                       .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByMonthAsync(Guid userProfileId, int year, int month, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1);
        return await db.Transactions
                       .Where(x => x.UserProfileId == userProfileId && x.Date >= from && x.Date < to)
                       .Include(x => x.Account)
                       .Include(x => x.Category)
                       .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
                       .OrderByDescending(x => x.Date)
                       .ToListAsync(ct);
    }

    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(ct);
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.Transactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
