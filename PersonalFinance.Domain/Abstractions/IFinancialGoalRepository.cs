using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Domain.Abstractions;

public interface IFinancialGoalRepository
{
    Task<FinancialGoal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FinancialGoal>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<FinancialGoal> AddAsync(FinancialGoal goal, CancellationToken ct = default);
    Task UpdateAsync(FinancialGoal goal, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
