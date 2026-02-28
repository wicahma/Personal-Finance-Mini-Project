using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly FinanceDbContext _db;

    public UserProfileRepository(FinanceDbContext db) => _db = db;

    public Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
        => _db.UserProfiles.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId, ct);

    public async Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken ct = default)
    {
        _db.UserProfiles.Add(userProfile);
        await _db.SaveChangesAsync(ct);
        return userProfile;
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken ct = default)
    {
        _db.UserProfiles.Update(userProfile);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
