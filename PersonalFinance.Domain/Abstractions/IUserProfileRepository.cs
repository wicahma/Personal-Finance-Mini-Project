using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Domain.Abstractions;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);
    Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken ct = default);
    Task UpdateAsync(UserProfile userProfile, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
