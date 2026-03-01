using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface IReportService
{
    Task<DashboardSummaryDto> GetSummaryAsync(Guid userProfileId, string month, CancellationToken ct = default);
    Task<IReadOnlyList<ChartDataDto>> GetChartDataAsync(Guid userProfileId, string month, CancellationToken ct = default);
}
