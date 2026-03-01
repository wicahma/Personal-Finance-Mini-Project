using System.Net.Http.Json;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpGoalClientService(HttpClient http) : IGoalClientService
{
    public async Task<IReadOnlyList<FinancialGoalModel>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<FinancialGoalModel>>>("api/goals", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<FinancialGoalModel>();
        }
        catch
        {
            return Array.Empty<FinancialGoalModel>();
        }
    }

    public async Task<FinancialGoalModel?> CreateAsync(CreateGoalRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PostAsJsonAsync("api/goals", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<FinancialGoalModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<FinancialGoalModel?> UpdateAsync(Guid id, UpdateGoalRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync($"api/goals/{id}", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<FinancialGoalModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var httpResponse = await http.DeleteAsync($"api/goals/{id}", ct);
        return httpResponse.IsSuccessStatusCode;
    }
}
