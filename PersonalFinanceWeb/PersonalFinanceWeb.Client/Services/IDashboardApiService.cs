using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface IDashboardApiService
{
    Task<DashboardSummaryModel?> GetSummaryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<ChartDataModel>> GetChartDataAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TagModel>> GetTagsAsync(CancellationToken ct = default);
}
