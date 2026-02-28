using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface IProfileClientService
{
    Task<UserProfileModel?> GetProfileAsync(CancellationToken ct = default);
    Task<UserProfileModel?> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default);
}
