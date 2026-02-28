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

    // GET /api/reports/summary?month=YYYY-MM
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] string? month, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var effectiveMonth = month ?? DateTime.UtcNow.ToString("yyyy-MM");
        var summary = await _reportService.GetSummaryAsync(profileId, effectiveMonth, ct);
        return OkData(summary);
    }

    // GET /api/reports/charts?month=YYYY-MM
    [HttpGet("charts")]
    public async Task<IActionResult> GetCharts([FromQuery] string? month, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var effectiveMonth = month ?? DateTime.UtcNow.ToString("yyyy-MM");
        var charts = await _reportService.GetChartDataAsync(profileId, effectiveMonth, ct);
        return OkData(charts);
    }
}
