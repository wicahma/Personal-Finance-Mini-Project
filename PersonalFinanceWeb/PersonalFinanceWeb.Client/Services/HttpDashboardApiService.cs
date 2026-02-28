using System.Net.Http.Json;
using System.Text;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpDashboardApiService : IDashboardApiService
{
    private readonly HttpClient _http;

    public HttpDashboardApiService(HttpClient http) => _http = http;

    public async Task<DashboardSummaryModel?> GetSummaryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default)
    {
        var url = BuildUrl("api/reports/summary", startDate, endDate, accountId, categoryId, tagId);
        var response = await _http.GetFromJsonAsync<ApiResponse<DashboardSummaryModel>>(url, ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<IReadOnlyList<ChartDataModel>> GetChartDataAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default)
    {
        var url = BuildUrl("api/reports/charts", startDate, endDate, accountId, categoryId, tagId);
        var response = await _http.GetFromJsonAsync<ApiResponse<List<ChartDataModel>>>(url, ct);
        return response?.Status == true && response.Data is not null
            ? response.Data
            : Array.Empty<ChartDataModel>();
    }

    public async Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<AccountModel>>>("api/accounts", ct);
        return response?.Status == true && response.Data is not null
            ? response.Data
            : Array.Empty<AccountModel>();
    }

    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<CategoryModel>>>("api/categories", ct);
        return response?.Status == true && response.Data is not null
            ? response.Data
            : Array.Empty<CategoryModel>();
    }

    public async Task<IReadOnlyList<TagModel>> GetTagsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<TagModel>>>("api/tags", ct);
        return response?.Status == true && response.Data is not null
            ? response.Data
            : Array.Empty<TagModel>();
    }

    private static string BuildUrl(
        string endpoint,
        DateTime? startDate,
        DateTime? endDate,
        Guid? accountId,
        Guid? categoryId,
        Guid? tagId)
    {
        var sb = new StringBuilder(endpoint);
        var hasQuery = false;

        void Append(string key, string value)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(key).Append('=').Append(Uri.EscapeDataString(value));
            hasQuery = true;
        }

        if (startDate.HasValue) Append("startDate", startDate.Value.ToString("yyyy-MM-dd"));
        if (endDate.HasValue) Append("endDate", endDate.Value.ToString("yyyy-MM-dd"));
        if (accountId.HasValue) Append("accountId", accountId.Value.ToString());
        if (categoryId.HasValue) Append("categoryId", categoryId.Value.ToString());
        if (tagId.HasValue) Append("tagId", tagId.Value.ToString());

        return sb.ToString();
    }
}
