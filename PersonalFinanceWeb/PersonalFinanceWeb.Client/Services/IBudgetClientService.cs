using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface IBudgetClientService
{
    Task<IReadOnlyList<BudgetModel>> GetByMonthAsync(string month, CancellationToken ct = default);
    Task<BudgetModel?> CreateAsync(CreateBudgetRequest request, CancellationToken ct = default);
    Task<BudgetModel?> UpdateAsync(Guid id, UpdateBudgetRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
