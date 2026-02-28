using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Services;

public sealed class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _repo;

    public UserProfileService(IUserProfileRepository repo) => _repo = repo;

    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdentityUserIdAsync(identityUserId, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<UserProfileDto> EnsureCreatedAsync(string identityUserId, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdentityUserIdAsync(identityUserId, ct);
        if (existing is not null) return MapToDto(existing);

        var newProfile = new UserProfile
        {
            IdentityUserId = identityUserId,
            DisplayName = string.Empty,
            DefaultCurrency = "USD",
            CreatedBy = identityUserId
        };
        await _repo.AddAsync(newProfile, ct);
        return MapToDto(newProfile);
    }

    public async Task<UserProfileDto> UpdateAsync(string identityUserId, UpdateProfileDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdentityUserIdAsync(identityUserId, ct)
                     ?? throw new KeyNotFoundException("User profile not found.");
        entity.DisplayName = dto.DisplayName;
        entity.DefaultCurrency = dto.DefaultCurrency;
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    private static UserProfileDto MapToDto(UserProfile e) => new(
        e.Id, e.IdentityUserId, e.DisplayName, e.DefaultCurrency, e.CreatedAt, e.UpdatedAt);
}
