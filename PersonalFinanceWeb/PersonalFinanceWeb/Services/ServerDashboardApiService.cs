using PersonalFinance.Application.Services;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerDashboardApiService : IDashboardApiService
{
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerDashboardApiService(
        IReportService reportService,
        IAccountService accountService,
        ICategoryService categoryService,
        ITagService tagService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _reportService = reportService;
        _accountService = accountService;
        _categoryService = categoryService;
        _tagService = tagService;
        _profileService = profileService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<Guid?> TryGetProfileIdAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return null;
        var profile = await _profileService.EnsureCreatedAsync(userId, ct);
        return profile.Id;
    }

    private static string MonthFromRange(DateTime? start, DateTime? end)
    {
        var d = start ?? end ?? DateTime.UtcNow;
        return d.ToString("yyyy-MM");
    }

    public async Task<DashboardSummaryModel?> GetSummaryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;

        var month = MonthFromRange(startDate, endDate);
        var result = await _reportService.GetSummaryAsync(profileId.Value, month, ct);
        return new DashboardSummaryModel
        {
            Month = result.Month,
            TotalIncome = result.TotalIncome,
            TotalExpense = result.TotalExpense,
            NetCashFlow = result.NetCashFlow
        };
    }

    public async Task<IReadOnlyList<ChartDataModel>> GetChartDataAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<ChartDataModel>();

        var month = MonthFromRange(startDate, endDate);
        var results = await _reportService.GetChartDataAsync(profileId.Value, month, ct);
        return results.Select(r => new ChartDataModel
        {
            CategoryId = r.CategoryId,
            CategoryName = r.CategoryName,
            IconOrColor = r.IconOrColor,
            TotalSpent = r.TotalSpent,
            Percentage = r.Percentage
        }).ToList();
    }

    public async Task<IReadOnlyList<AccountModel>> GetAccountsAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<AccountModel>();
        var accounts = await _accountService.GetByUserProfileIdAsync(profileId.Value, ct);
        return accounts.Select(a => new AccountModel { Id = a.Id, Name = a.Name }).ToList();
    }

    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<CategoryModel>();
        var categories = await _categoryService.GetByUserProfileIdAsync(profileId.Value, ct);
        return categories.Select(c => new CategoryModel { Id = c.Id, Name = c.Name }).ToList();
    }

    public async Task<IReadOnlyList<TagModel>> GetTagsAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<TagModel>();
        var tags = await _tagService.GetByUserProfileIdAsync(profileId.Value, ct);
        return tags.Select(t => new TagModel { Id = t.Id, Name = t.Name }).ToList();
    }
}
