using System.Net.Http.Json;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpBudgetClientService(HttpClient http) : IBudgetClientService
{
    public async Task<IReadOnlyList<BudgetModel>> GetByMonthAsync(string month, CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<BudgetModel>>>($"api/budgets?month={Uri.EscapeDataString(month)}", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<BudgetModel>();
        }
        catch
        {
            return Array.Empty<BudgetModel>();
        }
    }

    public async Task<BudgetModel?> CreateAsync(CreateBudgetRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PostAsJsonAsync("api/budgets", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<BudgetModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<BudgetModel?> UpdateAsync(Guid id, UpdateBudgetRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync($"api/budgets/{id}", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<BudgetModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var httpResponse = await http.DeleteAsync($"api/budgets/{id}", ct);
        return httpResponse.IsSuccessStatusCode;
    }
}
