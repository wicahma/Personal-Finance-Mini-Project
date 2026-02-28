using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class BudgetRepository : IBudgetRepository
{
    private readonly FinanceDbContext _db;

    public BudgetRepository(FinanceDbContext db) => _db = db;

    public Task<Budget?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Budgets.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Budget>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        => await _db.Budgets
                    .Where(x => x.UserProfileId == userProfileId)
                    .Include(x => x.Category)
                    .ToListAsync(ct);

    public async Task<IReadOnlyList<Budget>> GetByMonthAsync(Guid userProfileId, string month, CancellationToken ct = default)
        => await _db.Budgets
                    .Where(x => x.UserProfileId == userProfileId && x.Month == month)
                    .Include(x => x.Category)
                    .ToListAsync(ct);

    public Task<Budget?> GetByCategoryAndMonthAsync(Guid userProfileId, Guid categoryId, string month, CancellationToken ct = default)
        => _db.Budgets.FirstOrDefaultAsync(
            x => x.UserProfileId == userProfileId && x.CategoryId == categoryId && x.Month == month, ct);

    public async Task<Budget> AddAsync(Budget budget, CancellationToken ct = default)
    {
        _db.Budgets.Add(budget);
        await _db.SaveChangesAsync(ct);
        return budget;
    }

    public async Task UpdateAsync(Budget budget, CancellationToken ct = default)
    {
        _db.Budgets.Update(budget);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Budgets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
