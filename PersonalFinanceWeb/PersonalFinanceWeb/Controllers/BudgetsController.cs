using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class BudgetsController : BaseApiController
{
    private readonly IBudgetService _budgetService;
    private readonly IUserProfileService _profileService;

    public BudgetsController(IBudgetService budgetService, IUserProfileService profileService)
    {
        _budgetService = budgetService;
        _profileService = profileService;
    }

    // GET /api/budgets?month=YYYY-MM
    [HttpGet]
    public async Task<IActionResult> GetBudgets([FromQuery] string? month, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var effectiveMonth = month ?? DateTime.UtcNow.ToString("yyyy-MM");
        var budgets = await _budgetService.GetRealizationsAsync(profileId, effectiveMonth, ct);
        return OkData(budgets);
    }

    // POST /api/budgets
    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateBudgetDto(profileId, req.CategoryId, req.Month, req.Amount);
        var budget = await _budgetService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetBudgetById), new { id = budget.Id }, budget, "Budget created successfully.");
    }

    // GET /api/budgets/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBudgetById(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var budget = await _budgetService.GetByIdAsync(id, ct);
        if (budget is null || budget.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Budget not found."));
        return OkData(budget);
    }

    // PUT /api/budgets/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _budgetService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Budget not found."));

        var dto = new UpdateBudgetDto(id, req.Amount);
        var budget = await _budgetService.UpdateAsync(dto, ct);
        return OkData(budget, "Budget updated successfully.");
    }

    // DELETE /api/budgets/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _budgetService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Budget not found."));

        await _budgetService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateBudgetRequest(Guid CategoryId, string Month, decimal Amount);
public record UpdateBudgetRequest(decimal Amount);
