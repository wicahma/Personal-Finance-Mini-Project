using System.Security.Claims;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerTransactionClientService : ITransactionClientService
{
    private readonly ITransactionService _transactionService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerTransactionClientService(
        ITransactionService transactionService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _transactionService = transactionService;
        _profileService = profileService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<Guid?> TryGetProfileIdAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return null;
        var profile = await _profileService.EnsureCreatedAsync(userId, ct);
        return profile.Id;
    }

    private static TagItem MapTag(TagDto t) => new() { Id = t.Id, Name = t.Name, Color = t.Color };

    private static TransactionListItem MapToListItem(TransactionDto t) => new()
    {
        Id = t.Id,
        AccountId = t.AccountId,
        AccountName = t.AccountName,
        CategoryId = t.CategoryId,
        CategoryName = t.CategoryName,
        Amount = t.Amount,
        Date = t.Date,
        Notes = t.Notes,
        Type = t.Type.ToString(),
        IsTransfer = t.IsTransfer,
        TransferPairId = t.TransferPairId,
        Tags = t.Tags.Select(MapTag).ToList(),
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    public async Task<(List<TransactionListItem> Items, PaginationMeta? Pagination)> GetPagedAsync(
        TransactionFilterModel filter, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return ([], null);

        var query = new TransactionQueryDto(
            filter.StartDate, filter.EndDate,
            filter.AccountId, filter.CategoryId, filter.TagId,
            filter.Page, filter.PageSize);

        var paged = await _transactionService.GetPagedAsync(profileId.Value, query, ct);

        var pagination = new PaginationMeta
        {
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize,
            TotalItems = paged.TotalItems,
            TotalPages = paged.TotalPages
        };

        return (paged.Items.Select(MapToListItem).ToList(), pagination);
    }

    public async Task<TransactionDetailModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var detail = await _transactionService.GetDetailByIdAsync(id, ct);
        if (detail is null) return null;

        return new TransactionDetailModel
        {
            Id = detail.Id,
            AccountId = detail.AccountId,
            AccountName = detail.AccountName,
            CategoryId = detail.CategoryId,
            CategoryName = detail.CategoryName,
            CategoryIconOrColor = detail.CategoryIconOrColor,
            Amount = detail.Amount,
            Date = detail.Date,
            Notes = detail.Notes,
            Type = detail.Type.ToString(),
            IsTransfer = detail.IsTransfer,
            TransferPairId = detail.TransferPairId,
            Tags = detail.Tags.Select(MapTag).ToList(),
            CreatedAt = detail.CreatedAt,
            UpdatedAt = detail.UpdatedAt
        };
    }

    public async Task<TransactionListItem?> CreateAsync(CreateTransactionRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;

        if (!Enum.TryParse<TransactionType>(request.Type, out var txType))
            return null;

        var dto = new CreateTransactionDto(
            profileId.Value,
            request.AccountId,
            request.CategoryId,
            request.Amount,
            request.Date,
            request.Notes,
            txType,
            request.TagIds.Count > 0 ? request.TagIds : null);

        var tx = await _transactionService.CreateAsync(dto, ct);
        return MapToListItem(tx);
    }

    public async Task<bool> CreateTransferAsync(CreateTransferRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return false;

        var dto = new CreateTransferDto(
            profileId.Value,
            request.FromAccountId,
            request.ToAccountId,
            request.Amount,
            request.Date,
            request.Notes);

        await _transactionService.CreateTransferAsync(dto, ct);
        return true;
    }

    public async Task<TransactionListItem?> UpdateAsync(Guid id, UpdateTransactionRequest request, CancellationToken ct = default)
    {
        var existing = await _transactionService.GetByIdAsync(id, ct);
        if (existing is null) return null;

        var txType = !string.IsNullOrEmpty(request.Type) && Enum.TryParse<TransactionType>(request.Type, out var parsed)
            ? parsed
            : existing.Type;

        var dto = new UpdateTransactionDto(
            id,
            request.CategoryId,
            request.Amount,
            request.Date,
            request.Notes ?? existing.Notes,
            txType,
            request.TagIds);

        var tx = await _transactionService.UpdateAsync(dto, ct);
        return MapToListItem(tx);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _transactionService.DeleteAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
