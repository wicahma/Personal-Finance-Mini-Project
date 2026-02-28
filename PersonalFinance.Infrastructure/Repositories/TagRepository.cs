using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class TagRepository : ITagRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public TagRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Tags.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Tag>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Tags.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);
    }

    public async Task<Tag> AddAsync(Tag tag, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Tags.Add(tag);
        await db.SaveChangesAsync(ct);
        return tag;
    }

    public async Task UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Tags.Update(tag);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.Tags.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
