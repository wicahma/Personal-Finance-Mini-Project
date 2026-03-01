using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface ITagService
{
    Task<TagDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TagDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken ct = default);
    Task<TagDto> UpdateAsync(UpdateTagDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
