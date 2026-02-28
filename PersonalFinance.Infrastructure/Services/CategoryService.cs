using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo) => _repo = repo;

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetByTypeAsync(Guid userProfileId, CategoryType type, CancellationToken ct = default)
    {
        var entities = await _repo.GetByTypeAsync(userProfileId, type, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var entity = new Category
        {
            UserProfileId = dto.UserProfileId,
            Name = dto.Name,
            Type = dto.Type,
            IconOrColor = dto.IconOrColor,
            CreatedBy = dto.UserProfileId.ToString()
        };
        await _repo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Category {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.Type = dto.Type;
        entity.IconOrColor = dto.IconOrColor;
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static CategoryDto MapToDto(Category e) => new(
        e.Id, e.UserProfileId, e.Name, e.Type, e.IconOrColor, e.CreatedAt, e.UpdatedAt);
}
