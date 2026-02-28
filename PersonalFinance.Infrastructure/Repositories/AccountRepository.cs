using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly FinanceDbContext _db;

    public AccountRepository(FinanceDbContext db) => _db = db;

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Account>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        => await _db.Accounts.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);

    public async Task<Account> AddAsync(Account account, CancellationToken ct = default)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(ct);
        return account;
    }

    public async Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
