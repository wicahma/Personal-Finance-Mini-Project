using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly FinanceDbContext _db;

    public CategoryRepository(FinanceDbContext db) => _db = db;

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Category>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        => await _db.Categories.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);

    public async Task<IReadOnlyList<Category>> GetByTypeAsync(Guid userProfileId, CategoryType type, CancellationToken ct = default)
        => await _db.Categories.Where(x => x.UserProfileId == userProfileId && x.Type == type).ToListAsync(ct);

    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
