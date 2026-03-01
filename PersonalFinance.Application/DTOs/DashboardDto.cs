namespace PersonalFinance.Application.DTOs;

public record DashboardSummaryDto(
    string Month,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal NetCashFlow);

public record ChartDataDto(
    Guid CategoryId,
    string CategoryName,
    string IconOrColor,
    decimal TotalSpent,
    double Percentage);
