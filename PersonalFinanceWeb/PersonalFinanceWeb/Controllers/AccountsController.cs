using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class AccountsController : BaseApiController
{
    private readonly IAccountService _accountService;
    private readonly IUserProfileService _profileService;

    public AccountsController(IAccountService accountService, IUserProfileService profileService)
    {
        _accountService = accountService;
        _profileService = profileService;
    }

    // GET /api/accounts
    [HttpGet]
    public async Task<IActionResult> GetAccounts(CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var accounts = await _accountService.GetByUserProfileIdAsync(profileId, ct);
        return OkData(accounts);
    }

    // POST /api/accounts
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateAccountDto(profileId, req.Name, req.Type, req.InitialBalance);
        var account = await _accountService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetAccountById), new { id = account.Id }, account, "Account created successfully.");
    }

    // GET /api/accounts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var account = await _accountService.GetByIdAsync(id, ct);
        if (account is null || account.UserProfileId != profileId)
            return NotFound(PersonalFinance.Application.Common.ApiResponse<object>.Failure("Account not found."));
        return OkData(account);
    }

    // PUT /api/accounts/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _accountService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(PersonalFinance.Application.Common.ApiResponse<object>.Failure("Account not found."));

        var dto = new UpdateAccountDto(id, req.Name, req.IsArchived);
        var account = await _accountService.UpdateAsync(dto, ct);
        return OkData(account, "Account updated successfully.");
    }

    // PATCH /api/accounts/{id}/archive
    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveAccount(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _accountService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(PersonalFinance.Application.Common.ApiResponse<object>.Failure("Account not found."));

        await _accountService.ArchiveAccountAsync(id, ct);
        return OkData<object?>(null, "Account archived successfully.");
    }

    // DELETE /api/accounts/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _accountService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(PersonalFinance.Application.Common.ApiResponse<object>.Failure("Account not found."));

        await _accountService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateAccountRequest(string Name, AccountType Type, decimal InitialBalance = 0m);
public record UpdateAccountRequest(string Name, bool IsArchived);
