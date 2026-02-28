using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public CategoryRepository(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Category>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Categories.Where(x => x.UserProfileId == userProfileId).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetByTypeAsync(Guid userProfileId, CategoryType type, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Categories.Where(x => x.UserProfileId == userProfileId && x.Type == type).ToListAsync(ct);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Categories.Update(category);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var entity = await db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync(ct);
        }
    }
}
