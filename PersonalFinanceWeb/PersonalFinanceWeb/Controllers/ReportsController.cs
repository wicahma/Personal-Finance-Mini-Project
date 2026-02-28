using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Services;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
[Route("api/reports")]
public sealed class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;
    private readonly IUserProfileService _profileService;

    public ReportsController(IReportService reportService, IUserProfileService profileService)
    {
        _reportService = reportService;
        _profileService = profileService;
    }

    // GET /api/reports/summary?month=YYYY-MM  OR  ?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? month,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var effectiveMonth = ResolveMonth(month, startDate, endDate);
        var summary = await _reportService.GetSummaryAsync(profileId, effectiveMonth, ct);
        return OkData(summary);
    }

    // GET /api/reports/charts?month=YYYY-MM  OR  ?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
    [HttpGet("charts")]
    public async Task<IActionResult> GetCharts(
        [FromQuery] string? month,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var effectiveMonth = ResolveMonth(month, startDate, endDate);
        var charts = await _reportService.GetChartDataAsync(profileId, effectiveMonth, ct);
        return OkData(charts);
    }

    private static string ResolveMonth(string? month, DateTime? startDate, DateTime? endDate)
    {
        if (!string.IsNullOrWhiteSpace(month)) return month;
        var d = startDate ?? endDate ?? DateTime.UtcNow;
        return d.ToString("yyyy-MM");
    }
}
