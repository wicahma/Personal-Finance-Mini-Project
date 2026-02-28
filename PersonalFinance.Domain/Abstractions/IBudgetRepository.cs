using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Domain.Abstractions;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Budget>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<IReadOnlyList<Budget>> GetByMonthAsync(Guid userProfileId, string month, CancellationToken ct = default);
    Task<Budget?> GetByCategoryAndMonthAsync(Guid userProfileId, Guid categoryId, string month, CancellationToken ct = default);
    Task<Budget> AddAsync(Budget budget, CancellationToken ct = default);
    Task UpdateAsync(Budget budget, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
