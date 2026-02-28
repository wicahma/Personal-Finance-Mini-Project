using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Services;

public sealed class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo) => _repo = repo;

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IReadOnlyList<AccountDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken ct = default)
    {
        var entity = new Account
        {
            UserProfileId = dto.UserProfileId,
            Name = dto.Name,
            Type = dto.Type,
            CurrentBalance = dto.InitialBalance,
            CreatedBy = dto.UserProfileId.ToString()
        };
        await _repo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<AccountDto> UpdateAsync(UpdateAccountDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Account {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.Type = dto.Type;
        entity.IsArchived = dto.IsArchived;
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static AccountDto MapToDto(Account e) => new(
        e.Id, e.UserProfileId, e.Name, e.Type,
        e.CurrentBalance, e.IsArchived, e.CreatedAt, e.UpdatedAt);
}
