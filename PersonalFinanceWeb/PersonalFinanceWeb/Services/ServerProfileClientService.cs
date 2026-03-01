using System.Security.Claims;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerProfileClientService(
    IUserProfileService profileService,
    IHttpContextAccessor httpContextAccessor) : IProfileClientService
{
    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

    public async Task<UserProfileModel?> GetProfileAsync(CancellationToken ct = default)
    {
        var userId = GetUserId();
        var dto = await profileService.EnsureCreatedAsync(userId, ct);
        return MapToModel(dto);
    }

    public async Task<UserProfileModel?> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        var dto = await profileService.UpdateAsync(userId, new UpdateProfileDto(request.DisplayName, request.DefaultCurrency), ct);
        return MapToModel(dto);
    }

    private static UserProfileModel MapToModel(UserProfileDto dto) => new()
    {
        Id = dto.Id,
        IdentityUserId = dto.IdentityUserId,
        DisplayName = dto.DisplayName,
        DefaultCurrency = dto.DefaultCurrency,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
