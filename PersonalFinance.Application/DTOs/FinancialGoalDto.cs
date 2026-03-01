namespace PersonalFinance.Application.DTOs;

public record FinancialGoalDto(
    Guid Id,
    Guid UserProfileId,
    string Name,
    string? Description,
    decimal TargetAmount,
    decimal CurrentAmount,
    string DeadlineDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateFinancialGoalDto(
    Guid UserProfileId,
    string Name,
    string? Description,
    decimal TargetAmount,
    string DeadlineDate);

public record UpdateFinancialGoalDto(
    Guid Id,
    string Name,
    string? Description,
    decimal TargetAmount,
    decimal CurrentAmount,
    string DeadlineDate);
