using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface IFinancialGoalService
{
    Task<FinancialGoalDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FinancialGoalDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<FinancialGoalDto> CreateAsync(CreateFinancialGoalDto dto, CancellationToken ct = default);
    Task<FinancialGoalDto> UpdateAsync(UpdateFinancialGoalDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
