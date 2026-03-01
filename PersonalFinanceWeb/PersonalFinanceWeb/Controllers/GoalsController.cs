using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class GoalsController : BaseApiController
{
    private readonly IFinancialGoalService _goalService;
    private readonly IUserProfileService _profileService;

    public GoalsController(IFinancialGoalService goalService, IUserProfileService profileService)
    {
        _goalService = goalService;
        _profileService = profileService;
    }

    // GET /api/goals
    [HttpGet]
    public async Task<IActionResult> GetGoals(CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var goals = await _goalService.GetByUserProfileIdAsync(profileId, ct);
        return OkData(goals);
    }

    // GET /api/goals/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGoalById(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var goal = await _goalService.GetByIdAsync(id, ct);
        if (goal is null || goal.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Goal not found."));
        return OkData(goal);
    }

    // POST /api/goals
    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateFinancialGoalDto(profileId, req.Name, req.Description, req.TargetAmount, req.DeadlineDate);
        var goal = await _goalService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetGoalById), new { id = goal.Id }, goal, "Goal created successfully.");
    }

    // PUT /api/goals/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateGoalRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _goalService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Goal not found."));

        var dto = new UpdateFinancialGoalDto(id, req.Name, req.Description, req.TargetAmount, req.CurrentAmount, req.DeadlineDate);
        var goal = await _goalService.UpdateAsync(dto, ct);
        return OkData(goal, "Goal updated successfully.");
    }

    // DELETE /api/goals/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGoal(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _goalService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Goal not found."));

        await _goalService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateGoalRequest(string Name, string? Description, decimal TargetAmount, string DeadlineDate);
public record UpdateGoalRequest(string Name, string? Description, decimal TargetAmount, decimal CurrentAmount, string DeadlineDate);
