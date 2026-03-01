using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Services;

public sealed class ReportService : IReportService
{
    private readonly IDbContextFactory<FinanceDbContext> _factory;

    public ReportService(IDbContextFactory<FinanceDbContext> factory) => _factory = factory;

    public async Task<DashboardSummaryDto> GetSummaryAsync(Guid userProfileId, string month, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var (from, to) = ParseMonthRange(month);

        var transactions = await db.Transactions
            .Where(x => x.UserProfileId == userProfileId && x.Date >= from && x.Date < to && !x.IsTransfer)
            .ToListAsync(ct);

        var totalIncome = transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
        var totalExpense = transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);

        return new DashboardSummaryDto(month, totalIncome, totalExpense, totalIncome - totalExpense);
    }

    public async Task<IReadOnlyList<ChartDataDto>> GetChartDataAsync(Guid userProfileId, string month, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var (from, to) = ParseMonthRange(month);

        var grouped = await db.Transactions
            .Where(x => x.UserProfileId == userProfileId
                     && x.Date >= from && x.Date < to
                     && x.Type == TransactionType.Expense
                     && x.CategoryId != null)
            .GroupBy(x => x.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key!.Value,
                Total = g.Sum(x => x.Amount)
            })
            .ToListAsync(ct);

        if (grouped.Count == 0) return Array.Empty<ChartDataDto>();

        var totalSpent = grouped.Sum(g => g.Total);
        var categoryIds = grouped.Select(g => g.CategoryId).ToList();

        var categories = await db.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, ct);

        return grouped
            .OrderByDescending(g => g.Total)
            .Take(10)
            .Select(g => new ChartDataDto(
                g.CategoryId,
                categories.TryGetValue(g.CategoryId, out var cat) ? cat.Name : "Unknown",
                cat?.IconOrColor ?? string.Empty,
                g.Total,
                totalSpent == 0 ? 0 : Math.Round((double)g.Total / (double)totalSpent * 100, 2)))
            .ToList();
    }

    private static (DateTime From, DateTime To) ParseMonthRange(string month)
    {
        if (!DateTime.TryParseExact(month + "-01", "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var from))
        {
            var now = DateTime.UtcNow;
            from = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        else
        {
            from = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        }
        return (from, from.AddMonths(1));
    }
}
