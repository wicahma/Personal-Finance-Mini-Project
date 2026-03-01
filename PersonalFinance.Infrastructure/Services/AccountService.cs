using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Services;

public sealed class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;
    private readonly FinanceDbContext _db;

    public AccountService(IAccountRepository repo, FinanceDbContext db)
    {
        _repo = repo;
        _db = db;
    }

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
        await using var dbTx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var entity = new Account
            {
                UserProfileId = dto.UserProfileId,
                Name = dto.Name,
                Type = dto.Type,
                CurrentBalance = 0m,
                CreatedBy = dto.UserProfileId.ToString()
            };

            _db.Accounts.Add(entity);
            await _db.SaveChangesAsync(ct);

            if (dto.InitialBalance > 0m)
            {
                var initialTx = new Transaction
                {
                    UserProfileId = dto.UserProfileId,
                    AccountId = entity.Id,
                    CategoryId = null,
                    Amount = dto.InitialBalance,
                    Date = DateTime.UtcNow,
                    Notes = "Initial Balance",
                    Type = TransactionType.Income,
                    IsTransfer = false,
                    CreatedBy = dto.UserProfileId.ToString()
                };

                entity.CurrentBalance = dto.InitialBalance;
                _db.Transactions.Add(initialTx);
                await _db.SaveChangesAsync(ct);
            }

            await dbTx.CommitAsync(ct);
            return MapToDto(entity);
        }
        catch
        {
            await dbTx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<AccountDto> UpdateAsync(UpdateAccountDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Account {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.IsArchived = dto.IsArchived;
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task ArchiveAccountAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException($"Account {id} not found.");
        entity.IsArchived = true;
        await _repo.UpdateAsync(entity, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static AccountDto MapToDto(Account e) => new(
        e.Id, e.UserProfileId, e.Name, e.Type,
        e.CurrentBalance, e.IsArchived, e.CreatedAt, e.UpdatedAt);
}
