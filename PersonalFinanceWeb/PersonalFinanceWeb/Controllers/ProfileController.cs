using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class ProfileController : BaseApiController
{
    private readonly IUserProfileService _profileService;

    public ProfileController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    // GET /api/profile
    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var identityUserId = GetUserId();
        var profile = await _profileService.EnsureCreatedAsync(identityUserId, ct);
        return OkData(profile);
    }

    // PUT /api/profile
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken ct)
    {
        var identityUserId = GetUserId();
        var profile = await _profileService.UpdateAsync(identityUserId, dto, ct);
        return OkData(profile, "Profile updated successfully.");
    }
}
