using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinanceWeb.Client.Models;
using PersonalFinanceWeb.Client.Services;

namespace PersonalFinanceWeb.Services;

public sealed class ServerGoalClientService : IGoalClientService
{
    private readonly IFinancialGoalService _goalService;
    private readonly IUserProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerGoalClientService(
        IFinancialGoalService goalService,
        IUserProfileService profileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _goalService = goalService;
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

    private static FinancialGoalModel MapToModel(FinancialGoalDto d) => new()
    {
        Id = d.Id,
        UserProfileId = d.UserProfileId,
        Name = d.Name,
        Description = d.Description,
        TargetAmount = d.TargetAmount,
        CurrentAmount = d.CurrentAmount,
        DeadlineDate = d.DeadlineDate,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };

    public async Task<IReadOnlyList<FinancialGoalModel>> GetAllAsync(CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return Array.Empty<FinancialGoalModel>();
        var goals = await _goalService.GetByUserProfileIdAsync(profileId.Value, ct);
        return goals.Select(MapToModel).ToList();
    }

    public async Task<FinancialGoalModel?> CreateAsync(CreateGoalRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        var dto = new CreateFinancialGoalDto(profileId.Value, request.Name, request.Description, request.TargetAmount, request.DeadlineDate);
        var goal = await _goalService.CreateAsync(dto, ct);
        return MapToModel(goal);
    }

    public async Task<FinancialGoalModel?> UpdateAsync(Guid id, UpdateGoalRequest request, CancellationToken ct = default)
    {
        var profileId = await TryGetProfileIdAsync(ct);
        if (profileId is null) return null;
        var dto = new UpdateFinancialGoalDto(id, request.Name, request.Description, request.TargetAmount, request.CurrentAmount, request.DeadlineDate);
        var goal = await _goalService.UpdateAsync(dto, ct);
        return MapToModel(goal);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _goalService.DeleteAsync(id, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
