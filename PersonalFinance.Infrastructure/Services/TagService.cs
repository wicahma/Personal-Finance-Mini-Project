using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Services;

public sealed class TagService : ITagService
{
    private readonly ITagRepository _repo;

    public TagService(ITagRepository repo) => _repo = repo;

    public async Task<TagDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IReadOnlyList<TagDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken ct = default)
    {
        var entity = new Tag
        {
            UserProfileId = dto.UserProfileId,
            Name = dto.Name,
            Color = dto.Color,
            CreatedBy = dto.UserProfileId.ToString()
        };
        await _repo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<TagDto> UpdateAsync(UpdateTagDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Tag {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.Color = dto.Color;
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static TagDto MapToDto(Tag e) => new(
        e.Id, e.UserProfileId, e.Name, e.Color, e.CreatedAt, e.UpdatedAt);
}
