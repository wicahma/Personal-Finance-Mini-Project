using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerCategoryClientService : ICategoryClientService
{
    private readonly ICategoryService _categoryService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerCategoryClientService(
        ICategoryService categoryService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _categoryService = categoryService;
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

    private static CategoryManagementModel MapToModel(CategoryDto c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Type = c.Type.ToString(),
        IconOrColor = c.IconOrColor,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    public async Task<IReadOnlyList<CategoryManagementModel>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<CategoryManagementModel>();
        var categories = await _categoryService.GetByUserProfileIdAsync(profileId.Value, ct);
        return categories.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<CategoryManagementModel>> GetByTypeAsync(string type, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<CategoryManagementModel>();
        if (!Enum.TryParse<CategoryType>(type, out var categoryType))
            return Array.Empty<CategoryManagementModel>();
        var categories = await _categoryService.GetByTypeAsync(profileId.Value, categoryType, ct);
        return categories.Select(MapToModel).ToList();
    }

    public async Task<CategoryManagementModel?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        if (!Enum.TryParse<CategoryType>(request.Type, out var categoryType))
            return null;
        var dto = new CreateCategoryDto(profileId.Value, request.Name, categoryType, request.IconOrColor);
        var category = await _categoryService.CreateAsync(dto, ct);
        return MapToModel(category);
    }

    public async Task<CategoryManagementModel?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var existing = await _categoryService.GetByIdAsync(id, ct);
        if (existing is null) return null;
        var dto = new UpdateCategoryDto(id, request.Name, existing.Type, request.IconOrColor ?? existing.IconOrColor);
        var category = await _categoryService.UpdateAsync(dto, ct);
        return MapToModel(category);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _categoryService.DeleteAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
