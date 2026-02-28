namespace PersonalFinanceWeb.Client.Models;

public sealed class DashboardSummaryModel
{
    public string Month { get; init; } = string.Empty;
    public decimal TotalIncome { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal NetCashFlow { get; init; }
}

public sealed class ChartDataModel
{
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string IconOrColor { get; init; } = string.Empty;
    public decimal TotalSpent { get; init; }
    public double Percentage { get; init; }
}

public sealed class AccountModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class CategoryModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class TagModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
