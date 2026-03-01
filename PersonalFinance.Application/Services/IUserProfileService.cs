using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);
    Task<UserProfileDto> EnsureCreatedAsync(string identityUserId, CancellationToken ct = default);
    Task<UserProfileDto> UpdateAsync(string identityUserId, UpdateProfileDto dto, CancellationToken ct = default);
}
