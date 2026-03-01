using System.Net.Http.Json;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpCategoryClientService(HttpClient http) : ICategoryClientService
{
    public async Task<IReadOnlyList<CategoryManagementModel>> GetCategoriesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<CategoryManagementModel>>>("api/categories", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<CategoryManagementModel>();
        }
        catch
        {
            return Array.Empty<CategoryManagementModel>();
        }
    }

    public async Task<IReadOnlyList<CategoryManagementModel>> GetByTypeAsync(string type, CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<CategoryManagementModel>>>($"api/categories?type={type}", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<CategoryManagementModel>();
        }
        catch
        {
            return Array.Empty<CategoryManagementModel>();
        }
    }

    public async Task<CategoryManagementModel?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PostAsJsonAsync("api/categories", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<CategoryManagementModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<CategoryManagementModel?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync($"api/categories/{id}", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<CategoryManagementModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var httpResponse = await http.DeleteAsync($"api/categories/{id}", ct);
        return httpResponse.IsSuccessStatusCode;
    }
}
