using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface IGoalClientService
{
    Task<IReadOnlyList<FinancialGoalModel>> GetAllAsync(CancellationToken ct = default);
    Task<FinancialGoalModel?> CreateAsync(CreateGoalRequest request, CancellationToken ct = default);
    Task<FinancialGoalModel?> UpdateAsync(Guid id, UpdateGoalRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
