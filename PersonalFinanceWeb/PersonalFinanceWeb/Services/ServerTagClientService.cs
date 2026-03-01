using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerTagClientService : ITagClientService
{
    private readonly ITagService _tagService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerTagClientService(
        ITagService tagService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _tagService = tagService;
        _profileService = profileService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<Guid?> TryGetProfileIdAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return null;
        var profile = await _profileService.EnsureCreatedAsync(userId, ct);
        return profile.Id;
    }

    private static TagManagementModel MapToModel(TagDto t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Color = t.Color,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    public async Task<IReadOnlyList<TagManagementModel>> GetTagsAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<TagManagementModel>();
        var tags = await _tagService.GetByUserProfileIdAsync(profileId.Value, ct);
        return tags.Select(MapToModel).ToList();
    }

    public async Task<TagManagementModel?> CreateTagAsync(CreateTagRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        var dto = new CreateTagDto(profileId.Value, request.Name, request.Color);
        var tag = await _tagService.CreateAsync(dto, ct);
        return MapToModel(tag);
    }

    public async Task<TagManagementModel?> UpdateTagAsync(Guid id, UpdateTagRequest request, CancellationToken ct = default)
    {
        var existing = await _tagService.GetByIdAsync(id, ct);
        if (existing is null) return null;
        var dto = new UpdateTagDto(id, request.Name, request.Color ?? existing.Color);
        var tag = await _tagService.UpdateAsync(dto, ct);
        return MapToModel(tag);
    }

    public async Task<bool> DeleteTagAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _tagService.DeleteAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
