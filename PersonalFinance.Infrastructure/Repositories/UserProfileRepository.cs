using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public UserProfileRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.UserProfiles.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId, ct);
    }

    public async Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.UserProfiles.Add(userProfile);
        await db.SaveChangesAsync(ct);
        return userProfile;
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.UserProfiles.Update(userProfile);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
