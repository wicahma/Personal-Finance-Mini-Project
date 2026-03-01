using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerAccountClientService : IAccountClientService
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerAccountClientService(
        IAccountService accountService,
        ITransactionService transactionService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService;
        _transactionService = transactionService;
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

    private static AccountModel MapToModel(AccountDto a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Type = a.Type.ToString(),
        CurrentBalance = a.CurrentBalance,
        IsArchived = a.IsArchived,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };

    public async Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<AccountModel>();
        var accounts = await _accountService.GetByUserProfileIdAsync(profileId.Value, ct);
        return accounts.Select(MapToModel).ToList();
    }

    public async Task<AccountModel?> CreateAccountAsync(CreateAccountRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;

        if (!Enum.TryParse<PersonalFinance.Domain.Enums.AccountType>(request.Type, out var accountType))
            return null;

        var dto = new CreateAccountDto(profileId.Value, request.Name, accountType, request.InitialBalance);
        var account = await _accountService.CreateAsync(dto, ct);
        return MapToModel(account);
    }

    public async Task<AccountModel?> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken ct = default)
    {
        var dto = new UpdateAccountDto(id, request.Name, request.IsArchived);
        var account = await _accountService.UpdateAsync(dto, ct);
        return MapToModel(account);
    }

    public async Task<bool> ArchiveAccountAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _accountService.ArchiveAccountAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AdjustBalanceAsync(Guid accountId, decimal currentBalance, decimal targetBalance, CancellationToken ct = default)
    {
        var diff = targetBalance - currentBalance;
        if (diff == 0m) return true;

        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return false;

        var dto = new CreateTransactionDto(
            UserProfileId: profileId.Value,
            AccountId: accountId,
            CategoryId: null,
            Amount: Math.Abs(diff),
            Date: DateTime.UtcNow,
            Notes: "Balance Adjustment",
            Type: diff > 0 ? TransactionType.Income : TransactionType.Expense);

        try
        {
            await _transactionService.CreateAsync(dto, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
