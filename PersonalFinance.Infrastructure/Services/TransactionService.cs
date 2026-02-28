using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;
    private readonly FinanceDbContext _db;

    public TransactionService(ITransactionRepository repo, FinanceDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public async Task<TransactionDetailDto?> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Transactions
            .Include(x => x.Account)
            .Include(x => x.Category)
            .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : MapToDetailDto(e);
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<PagedList<TransactionDto>> GetPagedAsync(Guid userProfileId, TransactionQueryDto query, CancellationToken ct = default)
    {
        var q = _db.Transactions
            .Include(x => x.Account)
            .Include(x => x.Category)
            .Include(x => x.TransactionTags).ThenInclude(x => x.Tag)
            .Where(x => x.UserProfileId == userProfileId);

        if (query.StartDate.HasValue) q = q.Where(x => x.Date >= query.StartDate.Value);
        if (query.EndDate.HasValue) q = q.Where(x => x.Date <= query.EndDate.Value);
        if (query.AccountId.HasValue) q = q.Where(x => x.AccountId == query.AccountId.Value);
        if (query.CategoryId.HasValue) q = q.Where(x => x.CategoryId == query.CategoryId.Value);
        if (query.TagId.HasValue) q = q.Where(x => x.TransactionTags.Any(tt => tt.TagId == query.TagId.Value));

        var totalItems = await q.CountAsync(ct);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await q
            .OrderByDescending(x => x.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<TransactionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<IReadOnlyList<TransactionDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByAccountIdAsync(accountId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetByMonthAsync(Guid userProfileId, int year, int month, CancellationToken ct = default)
    {
        var entities = await _repo.GetByMonthAsync(userProfileId, year, month, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken ct = default)
    {
        await using var dbTx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == dto.AccountId, ct)
                          ?? throw new KeyNotFoundException($"Account {dto.AccountId} not found.");

            var transaction = new Transaction
            {
                UserProfileId = dto.UserProfileId,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Date = dto.Date,
                Notes = dto.Notes,
                Type = dto.Type,
                IsTransfer = false,
                CreatedBy = dto.UserProfileId.ToString()
            };

            account.CurrentBalance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;

            _db.Transactions.Add(transaction);

            if (dto.TagIds is { Count: > 0 })
                transaction.TransactionTags = dto.TagIds.Select(tid => new TransactionTag
                {
                    TransactionId = transaction.Id,
                    TagId = tid
                }).ToList();

            await _db.SaveChangesAsync(ct);
            await dbTx.CommitAsync(ct);

            return MapToDto(transaction);
        }
        catch
        {
            await dbTx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<(TransactionDto From, TransactionDto To)> CreateTransferAsync(CreateTransferDto dto, CancellationToken ct = default)
    {
        await using var dbTx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var fromAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == dto.FromAccountId, ct)
                              ?? throw new KeyNotFoundException($"Account {dto.FromAccountId} not found.");
            var toAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == dto.ToAccountId, ct)
                            ?? throw new KeyNotFoundException($"Account {dto.ToAccountId} not found.");

            var fromTx = new Transaction
            {
                UserProfileId = dto.UserProfileId,
                AccountId = dto.FromAccountId,
                Amount = dto.Amount,
                Date = dto.Date,
                Notes = dto.Notes,
                Type = TransactionType.Transfer,
                IsTransfer = true,
                CreatedBy = dto.UserProfileId.ToString()
            };

            var toTx = new Transaction
            {
                UserProfileId = dto.UserProfileId,
                AccountId = dto.ToAccountId,
                Amount = dto.Amount,
                Date = dto.Date,
                Notes = dto.Notes,
                Type = TransactionType.Transfer,
                IsTransfer = true,
                CreatedBy = dto.UserProfileId.ToString()
            };

            fromTx.TransferPairId = toTx.Id;
            toTx.TransferPairId = fromTx.Id;

            fromAccount.CurrentBalance -= dto.Amount;
            toAccount.CurrentBalance += dto.Amount;

            _db.Transactions.AddRange(fromTx, toTx);
            await _db.SaveChangesAsync(ct);
            await dbTx.CommitAsync(ct);

            return (MapToDto(fromTx), MapToDto(toTx));
        }
        catch
        {
            await dbTx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<TransactionDto> UpdateAsync(UpdateTransactionDto dto, CancellationToken ct = default)
    {
        await using var dbTx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var entity = await _db.Transactions
                                  .Include(x => x.TransactionTags)
                                  .FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                         ?? throw new KeyNotFoundException($"Transaction {dto.Id} not found.");

            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == entity.AccountId, ct)!;
            if (account is not null)
            {
                account.CurrentBalance -= entity.Type == TransactionType.Income ? entity.Amount : -entity.Amount;
                account.CurrentBalance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
            }

            entity.CategoryId = dto.CategoryId;
            entity.Amount = dto.Amount;
            entity.Date = dto.Date;
            entity.Notes = dto.Notes;
            entity.Type = dto.Type;

            _db.TransactionTags.RemoveRange(entity.TransactionTags);
            if (dto.TagIds is { Count: > 0 })
                entity.TransactionTags = dto.TagIds.Select(tid => new TransactionTag
                {
                    TransactionId = entity.Id,
                    TagId = tid
                }).ToList();

            await _db.SaveChangesAsync(ct);
            await dbTx.CommitAsync(ct);

            return MapToDto(entity);
        }
        catch
        {
            await dbTx.RollbackAsync(ct);
            throw;
        }
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static TransactionDto MapToDto(Transaction e)
    {
        var tags = e.TransactionTags
            .Select(tt => new TagDto(tt.Tag.Id, tt.Tag.UserProfileId, tt.Tag.Name, tt.Tag.Color,
                tt.Tag.CreatedAt, tt.Tag.UpdatedAt))
            .ToList();

        return new TransactionDto(
            e.Id, e.UserProfileId, e.AccountId, e.Account?.Name ?? string.Empty,
            e.CategoryId, e.Category?.Name,
            e.Amount, e.Date, e.Notes, e.Type,
            e.IsTransfer, e.TransferPairId,
            tags, e.CreatedAt, e.UpdatedAt);
    }

    private static TransactionDetailDto MapToDetailDto(Transaction e)
    {
        var tags = e.TransactionTags
            .Select(tt => new TagDto(tt.Tag.Id, tt.Tag.UserProfileId, tt.Tag.Name, tt.Tag.Color,
                tt.Tag.CreatedAt, tt.Tag.UpdatedAt))
            .ToList();

        return new TransactionDetailDto(
            e.Id, e.UserProfileId, e.AccountId, e.Account?.Name ?? string.Empty,
            e.CategoryId, e.Category?.Name, e.Category?.IconOrColor,
            e.Amount, e.Date, e.Notes, e.Type,
            e.IsTransfer, e.TransferPairId,
            tags, e.CreatedAt, e.UpdatedAt);
    }
}
