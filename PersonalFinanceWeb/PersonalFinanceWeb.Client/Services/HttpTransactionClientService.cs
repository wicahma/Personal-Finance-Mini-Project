using System.Net.Http.Json;
using System.Text;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpTransactionClientService(HttpClient http) : ITransactionClientService
{
    public async Task<(List<TransactionListItem> Items, PaginationMeta? Pagination)> GetPagedAsync(
        TransactionFilterModel filter, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl(filter);
            var response = await http.GetFromJsonAsync<ApiResponse<List<TransactionListItem>>>(url, ct);
            if (response?.Status == true && response.Data is not null)
                return (response.Data, response.Pagination);
            return ([], null);
        }
        catch
        {
            return ([], null);
        }
    }

    public async Task<TransactionDetailModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<TransactionDetailModel>>(
                $"api/transactions/{id}", ct);
            return response?.Status == true ? response.Data : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<TransactionListItem?> CreateAsync(CreateTransactionRequest request, CancellationToken ct = default)
    {
        try
        {
            var httpResponse = await http.PostAsJsonAsync("api/transactions", request, ct);
            if (!httpResponse.IsSuccessStatusCode) return null;
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionListItem>>(ct);
            return response?.Status == true ? response.Data : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateTransferAsync(CreateTransferRequest request, CancellationToken ct = default)
    {
        try
        {
            // Server's endpoint expects SourceAccountId / DestinationAccountId
            var httpResponse = await http.PostAsJsonAsync("api/transactions/transfer", new
            {
                sourceAccountId = request.FromAccountId,
                destinationAccountId = request.ToAccountId,
                amount = request.Amount,
                date = request.Date,
                notes = request.Notes
            }, ct);
            return httpResponse.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TransactionListItem?> UpdateAsync(Guid id, UpdateTransactionRequest request, CancellationToken ct = default)
    {
        try
        {
            var httpResponse = await http.PutAsJsonAsync($"api/transactions/{id}", request, ct);
            if (!httpResponse.IsSuccessStatusCode) return null;
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionListItem>>(ct);
            return response?.Status == true ? response.Data : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var httpResponse = await http.DeleteAsync($"api/transactions/{id}", ct);
            return httpResponse.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildUrl(TransactionFilterModel f)
    {
        var sb = new StringBuilder("api/transactions?page=");
        sb.Append(f.Page);
        sb.Append("&pageSize=");
        sb.Append(f.PageSize);
        if (f.StartDate.HasValue)
            sb.Append($"&startDate={Uri.EscapeDataString(f.StartDate.Value.ToString("O"))}");
        if (f.EndDate.HasValue)
            sb.Append($"&endDate={Uri.EscapeDataString(f.EndDate.Value.ToString("O"))}");
        if (f.AccountId.HasValue)
            sb.Append($"&accountId={f.AccountId.Value}");
        if (f.CategoryId.HasValue)
            sb.Append($"&categoryId={f.CategoryId.Value}");
        if (f.TagId.HasValue)
            sb.Append($"&tagId={f.TagId.Value}");
        return sb.ToString();
    }
}
