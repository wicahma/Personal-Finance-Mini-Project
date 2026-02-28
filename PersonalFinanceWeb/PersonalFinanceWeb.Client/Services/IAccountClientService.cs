using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface IAccountClientService
{
    Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default);
    Task<AccountModel?> CreateAccountAsync(CreateAccountRequest request, CancellationToken ct = default);
    Task<AccountModel?> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken ct = default);
    Task<bool> ArchiveAccountAsync(Guid id, CancellationToken ct = default);
    Task<bool> AdjustBalanceAsync(Guid accountId, decimal currentBalance, decimal targetBalance, CancellationToken ct = default);
}
