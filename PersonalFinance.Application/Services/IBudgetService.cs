using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface IBudgetService
{
    Task<BudgetDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BudgetDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<IReadOnlyList<BudgetDto>> GetByMonthAsync(Guid userProfileId, string month, CancellationToken ct = default);
    Task<IReadOnlyList<BudgetRealizationDto>> GetRealizationsAsync(Guid userProfileId, string month, CancellationToken ct = default);
    Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default);
    Task<BudgetDto> UpdateAsync(UpdateBudgetDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
