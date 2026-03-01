using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class BudgetRepository : IBudgetRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public BudgetRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Budgets.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Budget>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Budgets
                       .Where(x => x.UserProfileId == userProfileId)
                       .Include(x => x.Category)
                       .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Budget>> GetByMonthAsync(Guid userProfileId, string month, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Budgets
                       .Where(x => x.UserProfileId == userProfileId && x.Month == month)
                       .Include(x => x.Category)
                       .ToListAsync(ct);
    }

    public async Task<Budget?> GetByCategoryAndMonthAsync(Guid userProfileId, Guid categoryId, string month, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Budgets.FirstOrDefaultAsync(
            x => x.UserProfileId == userProfileId && x.CategoryId == categoryId && x.Month == month, ct);
    }

    public async Task<Budget> AddAsync(Budget budget, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Budgets.Add(budget);
        await db.SaveChangesAsync(ct);
        return budget;
    }

    public async Task UpdateAsync(Budget budget, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Budgets.Update(budget);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.Budgets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
