using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult OkData<T>(T data, string message = "Success", PaginationMeta? pagination = null)
        => Ok(ApiResponse<T>.Success(data, message, pagination));

    protected IActionResult CreatedData<T>(string actionName, object routeValues, T data, string message = "Created successfully")
        => CreatedAtAction(actionName, routeValues, ApiResponse<T>.Success(data, message));

    protected IActionResult NoContentResponse()
        => NoContent();

    protected string GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
        return id;
    }

    protected async Task<Guid> GetUserProfileIdAsync(IUserProfileService profileService, CancellationToken ct = default)
    {
        var identityUserId = GetUserId();
        var profile = await profileService.EnsureCreatedAsync(identityUserId, ct);
        return profile.Id;
    }
}
