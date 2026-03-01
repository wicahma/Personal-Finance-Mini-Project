using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public AccountRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Account>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Accounts.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);
        return account;
    }

    public async Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Accounts.Update(account);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
