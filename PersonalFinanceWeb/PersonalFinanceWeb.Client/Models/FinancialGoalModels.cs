namespace PersonalFinanceWeb.Client.Models;

public sealed class FinancialGoalModel
{
    public Guid Id { get; init; }
    public Guid UserProfileId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal TargetAmount { get; init; }
    public decimal CurrentAmount { get; init; }
    public string DeadlineDate { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public decimal RemainingAmount => TargetAmount - CurrentAmount;
    public double ProgressPercentage => TargetAmount == 0 ? 0 : Math.Round((double)CurrentAmount / (double)TargetAmount * 100, 2);
}

public sealed class CreateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public string DeadlineDate { get; set; } = string.Empty;
}

public sealed class UpdateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public string DeadlineDate { get; set; } = string.Empty;
}
