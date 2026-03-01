namespace PersonalFinanceWeb.Client.Models;

public sealed class BudgetModel
{
    public Guid Id { get; init; }
    public Guid UserProfileId { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIconOrColor { get; init; } = "#FFFFFF";
    public string Month { get; init; } = string.Empty;
    public decimal BudgetAmount { get; init; }
    public decimal SpentAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public double PercentageUsed { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class CreateBudgetRequest
{
    public Guid CategoryId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public sealed class UpdateBudgetRequest
{
    public decimal Amount { get; set; }
}
