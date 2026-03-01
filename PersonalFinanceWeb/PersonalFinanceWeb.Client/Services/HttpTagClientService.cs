using System.Net.Http.Json;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpTagClientService(HttpClient http) : ITagClientService
{
    public async Task<IReadOnlyList<TagManagementModel>> GetTagsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await http.GetFromJsonAsync<ApiResponse<List<TagManagementModel>>>("api/tags", ct);
            return response?.Status == true && response.Data is not null
                ? response.Data
                : Array.Empty<TagManagementModel>();
        }
        catch
        {
            return Array.Empty<TagManagementModel>();
        }
    }

    public async Task<TagManagementModel?> CreateTagAsync(CreateTagRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PostAsJsonAsync("api/tags", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TagManagementModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<TagManagementModel?> UpdateTagAsync(Guid id, UpdateTagRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync($"api/tags/{id}", request, ct);
        if (!httpResponse.IsSuccessStatusCode) return null;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TagManagementModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<bool> DeleteTagAsync(Guid id, CancellationToken ct = default)
    {
        var httpResponse = await http.DeleteAsync($"api/tags/{id}", ct);
        return httpResponse.IsSuccessStatusCode;
    }
}
