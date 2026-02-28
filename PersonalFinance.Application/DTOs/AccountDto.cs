using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public record AccountDto(
    Guid Id,
    Guid UserProfileId,
    string Name,
    AccountType Type,
    decimal CurrentBalance,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateAccountDto(
    Guid UserProfileId,
    string Name,
    AccountType Type,
    decimal InitialBalance = 0m);

public record UpdateAccountDto(
    Guid Id,
    string Name,
    bool IsArchived);
