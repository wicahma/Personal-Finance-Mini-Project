using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class FinancialGoalRepository : IFinancialGoalRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public FinancialGoalRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<FinancialGoal?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.FinancialGoals.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<FinancialGoal>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.FinancialGoals
                       .Where(x => x.UserProfileId == userProfileId)
                       .OrderBy(x => x.DeadlineDate)
                       .ToListAsync(ct);
    }

    public async Task<FinancialGoal> AddAsync(FinancialGoal goal, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.FinancialGoals.Add(goal);
        await db.SaveChangesAsync(ct);
        return goal;
    }

    public async Task UpdateAsync(FinancialGoal goal, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.FinancialGoals.Update(goal);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.FinancialGoals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
