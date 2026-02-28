using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AccountDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken ct = default);
    Task<AccountDto> UpdateAsync(UpdateAccountDto dto, CancellationToken ct = default);
    Task ArchiveAccountAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
