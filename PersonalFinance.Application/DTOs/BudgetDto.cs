namespace PersonalFinance.Application.DTOs;

public record BudgetDto(
    Guid Id,
    Guid UserProfileId,
    Guid CategoryId,
    string CategoryName,
    string Month,
    decimal Amount,
    decimal Spent,
    decimal Remaining,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateBudgetDto(
    Guid UserProfileId,
    Guid CategoryId,
    string Month,
    decimal Amount);

public record UpdateBudgetDto(
    Guid Id,
    decimal Amount);

public record BudgetRealizationDto(
    Guid Id,
    Guid UserProfileId,
    Guid CategoryId,
    string CategoryName,
    string CategoryIconOrColor,
    string Month,
    decimal BudgetAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    double PercentageUsed,
    DateTime CreatedAt,
    DateTime UpdatedAt);
