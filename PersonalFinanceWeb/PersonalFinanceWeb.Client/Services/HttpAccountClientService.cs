using System.Net.Http.Json;
using System.Text;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpAccountClientService(HttpClient http) : IAccountClientService
{
    public async Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<AccountModel>>>("api/accounts", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<AccountModel>();
        }
        catch
        {
            return Array.Empty<AccountModel>();
        }
    }

    public async Task<AccountModel?> CreateAccountAsync(CreateAccountRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PostAsJsonAsync("api/accounts", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AccountModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<AccountModel?> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync($"api/accounts/{id}", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AccountModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<bool> ArchiveAccountAsync(Guid id, CancellationToken ct = default)
    {
        var httpResponse = await http.PatchAsync(
            $"api/accounts/{id}/archive",
            new StringContent("{}", Encoding.UTF8, "application/json"),
            ct);
        return httpResponse.IsSuccessStatusCode;
    }

    public async Task<bool> AdjustBalanceAsync(Guid accountId, decimal currentBalance, decimal targetBalance, CancellationToken ct = default)
    {
        var diff = targetBalance - currentBalance;
        if (diff == 0m) return true;

        var body = new
        {
            accountId,
            categoryId = (Guid?)null,
            amount = Math.Abs(diff),
            date = DateTime.UtcNow,
            type = diff > 0 ? "Income" : "Expense",
            notes = "Balance Adjustment"
        };

        var httpResponse = await http.PostAsJsonAsync("api/transactions", body, ct);
        return httpResponse.IsSuccessStatusCode;
    }
}
