using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public record TransactionDto(
    Guid Id,
    Guid UserProfileId,
    Guid AccountId,
    string AccountName,
    Guid? CategoryId,
    string? CategoryName,
    decimal Amount,
    DateTime Date,
    string Notes,
    TransactionType Type,
    bool IsTransfer,
    Guid? TransferPairId,
    IReadOnlyList<TagDto> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateTransactionDto(
    Guid UserProfileId,
    Guid AccountId,
    Guid? CategoryId,
    decimal Amount,
    DateTime Date,
    string Notes,
    TransactionType Type,
    IList<Guid>? TagIds = null);

public record CreateTransferDto(
    Guid UserProfileId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    DateTime Date,
    string Notes);

public record UpdateTransactionDto(
    Guid Id,
    Guid? CategoryId,
    decimal Amount,
    DateTime Date,
    string Notes,
    TransactionType Type,
    IList<Guid>? TagIds = null);

public record TransactionDetailDto(
    Guid Id,
    Guid UserProfileId,
    Guid AccountId,
    string AccountName,
    Guid? CategoryId,
    string? CategoryName,
    string? CategoryIconOrColor,
    decimal Amount,
    DateTime Date,
    string Notes,
    TransactionType Type,
    bool IsTransfer,
    Guid? TransferPairId,
    IReadOnlyList<TagDto> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record TransactionQueryDto(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    Guid? TagId = null,
    int Page = 1,
    int PageSize = 20);
