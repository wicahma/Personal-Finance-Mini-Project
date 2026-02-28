using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class TagRepository : ITagRepository
{
    private readonly FinanceDbContext _db;

    public TagRepository(FinanceDbContext db) => _db = db;

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Tags.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Tag>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        => await _db.Tags.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);

    public async Task<Tag> AddAsync(Tag tag, CancellationToken ct = default)
    {
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(ct);
        return tag;
    }

    public async Task UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        _db.Tags.Update(tag);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
