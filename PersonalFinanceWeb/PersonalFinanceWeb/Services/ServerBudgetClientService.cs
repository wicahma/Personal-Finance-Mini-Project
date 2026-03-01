using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerBudgetClientService : IBudgetClientService
{
    private readonly IBudgetService _budgetService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerBudgetClientService(
        IBudgetService budgetService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _budgetService = budgetService;
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

    private static BudgetModel MapToModel(BudgetRealizationDto d) => new()
    {
        Id = d.Id,
        UserProfileId = d.UserProfileId,
        CategoryId = d.CategoryId,
        CategoryName = d.CategoryName,
        CategoryIconOrColor = d.CategoryIconOrColor,
        Month = d.Month,
        BudgetAmount = d.BudgetAmount,
        SpentAmount = d.SpentAmount,
        RemainingAmount = d.RemainingAmount,
        PercentageUsed = d.PercentageUsed,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };

    public async Task<IReadOnlyList<BudgetModel>> GetByMonthAsync(string month, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<BudgetModel>();
        var items = await _budgetService.GetRealizationsAsync(profileId.Value, month, ct);
        return items.Select(MapToModel).ToList();
    }

    public async Task<BudgetModel?> CreateAsync(CreateBudgetRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        var dto = new CreateBudgetDto(profileId.Value, request.CategoryId, request.Month, request.Amount);
        var budget = await _budgetService.CreateAsync(dto, ct);
        return MapBudgetDtoToModel(budget);
    }

    public async Task<BudgetModel?> UpdateAsync(Guid id, UpdateBudgetRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        var dto = new UpdateBudgetDto(id, request.Amount);
        var budget = await _budgetService.UpdateAsync(dto, ct);
        return MapBudgetDtoToModel(budget);
    }

    private static BudgetModel MapBudgetDtoToModel(BudgetDto b) => new()
    {
        Id = b.Id,
        UserProfileId = b.UserProfileId,
        CategoryId = b.CategoryId,
        CategoryName = b.CategoryName,
        Month = b.Month,
        BudgetAmount = b.Amount,
        SpentAmount = b.Spent,
        RemainingAmount = b.Remaining,
        PercentageUsed = b.Amount == 0 ? 0 : Math.Round((double)b.Spent / (double)b.Amount * 100, 2),
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _budgetService.DeleteAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
