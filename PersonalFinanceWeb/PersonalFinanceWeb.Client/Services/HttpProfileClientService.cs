using System.Net.Http.Json;
using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public sealed class HttpProfileClientService(HttpClient http) : IProfileClientService
{
    public async Task<UserProfileModel?> GetProfileAsync(CancellationToken ct = default)
    {
        var response = await http.GetFromJsonAsync<ApiResponse<UserProfileModel>>("api/profile", ct);
        return response?.Status == true ? response.Data : null;
    }

    public async Task<UserProfileModel?> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default)
    {
        var httpResponse = await http.PutAsJsonAsync("api/profile", request, ct);
        if (!httpResponse.IsSuccessStatusCode)
            return null;

        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<UserProfileModel>>(ct);
        return response?.Status == true ? response.Data : null;
    }
}
