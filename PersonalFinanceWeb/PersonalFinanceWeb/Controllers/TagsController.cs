using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class TagsController : BaseApiController
{
    private readonly ITagService _tagService;
    private readonly IUserProfileService _profileService;

    public TagsController(ITagService tagService, IUserProfileService profileService)
    {
        _tagService = tagService;
        _profileService = profileService;
    }

    // GET /api/tags
    [HttpGet]
    public async Task<IActionResult> GetTags(CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var tags = await _tagService.GetByUserProfileIdAsync(profileId, ct);
        return OkData(tags);
    }

    // POST /api/tags
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateTagDto(profileId, req.Name, req.Color ?? "#FFFFFF");
        var tag = await _tagService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetTagById), new { id = tag.Id }, tag, "Tag created successfully.");
    }

    // GET /api/tags/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTagById(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var tag = await _tagService.GetByIdAsync(id, ct);
        if (tag is null || tag.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Tag not found."));
        return OkData(tag);
    }

    // PUT /api/tags/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateTagRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _tagService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Tag not found."));

        var dto = new UpdateTagDto(id, req.Name, req.Color ?? existing.Color);
        var tag = await _tagService.UpdateAsync(dto, ct);
        return OkData(tag, "Tag updated successfully.");
    }

    // DELETE /api/tags/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _tagService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Tag not found."));

        await _tagService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateTagRequest(string Name, string? Color = null);
public record UpdateTagRequest(string Name, string? Color = null);
