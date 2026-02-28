using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;
    private readonly IUserProfileService _profileService;

    public CategoriesController(ICategoryService categoryService, IUserProfileService profileService)
    {
        _categoryService = categoryService;
        _profileService = profileService;
    }

    // GET /api/categories?type=Income|Expense
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryType? type, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        IReadOnlyList<CategoryDto> categories = type.HasValue
            ? await _categoryService.GetByTypeAsync(profileId, type.Value, ct)
            : await _categoryService.GetByUserProfileIdAsync(profileId, ct);
        return OkData(categories);
    }

    // POST /api/categories
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateCategoryDto(profileId, req.Name, req.Type, req.IconOrColor ?? string.Empty);
        var category = await _categoryService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetCategoryById), new { id = category.Id }, category, "Category created successfully.");
    }

    // GET /api/categories/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var category = await _categoryService.GetByIdAsync(id, ct);
        if (category is null || category.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Category not found."));
        return OkData(category);
    }

    // PUT /api/categories/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _categoryService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Category not found."));

        var dto = new UpdateCategoryDto(id, req.Name, existing.Type, req.IconOrColor ?? existing.IconOrColor);
        var category = await _categoryService.UpdateAsync(dto, ct);
        return OkData(category, "Category updated successfully.");
    }

    // DELETE /api/categories/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _categoryService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Category not found."));

        await _categoryService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateCategoryRequest(string Name, CategoryType Type, string? IconOrColor = null);
public record UpdateCategoryRequest(string Name, string? IconOrColor = null);
