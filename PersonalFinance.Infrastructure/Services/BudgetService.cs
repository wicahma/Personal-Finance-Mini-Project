using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Services;

public sealed class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _repo;
    private readonly FinanceDbContext _db;

    public BudgetService(IBudgetRepository repo, FinanceDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public async Task<BudgetDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e is null) return null;
        var spent = await ComputeSpentAsync(e.UserProfileId, e.CategoryId, e.Month, ct);
        return MapToDto(e, spent);
    }

    public async Task<IReadOnlyList<BudgetDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return await EnrichWithSpentAsync(entities, ct);
    }

    public async Task<IReadOnlyList<BudgetDto>> GetByMonthAsync(Guid userProfileId, string month, CancellationToken ct = default)
    {
        var entities = await _repo.GetByMonthAsync(userProfileId, month, ct);
        return await EnrichWithSpentAsync(entities, ct);
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default)
    {
        var entity = new Budget
        {
            UserProfileId = dto.UserProfileId,
            CategoryId = dto.CategoryId,
            Month = dto.Month,
            Amount = dto.Amount,
            CreatedBy = dto.UserProfileId.ToString()
        };
        await _repo.AddAsync(entity, ct);
        var fetched = (await _repo.GetByIdAsync(entity.Id, ct))!;
        var spent = await ComputeSpentAsync(fetched.UserProfileId, fetched.CategoryId, fetched.Month, ct);
        return MapToDto(fetched, spent);
    }

    public async Task<BudgetDto> UpdateAsync(UpdateBudgetDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Budget {dto.Id} not found.");
        entity.Amount = dto.Amount;
        await _repo.UpdateAsync(entity, ct);
        var spent = await ComputeSpentAsync(entity.UserProfileId, entity.CategoryId, entity.Month, ct);
        return MapToDto(entity, spent);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);


    private async Task<decimal> ComputeSpentAsync(Guid userProfileId, Guid categoryId, string month, CancellationToken ct)
    {
        if (!TryParseMonth(month, out int year, out int mon)) return 0m;
        var from = new DateTime(year, mon, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1);
        return await _db.Transactions
            .Where(x => x.UserProfileId == userProfileId
                     && x.CategoryId == categoryId
                     && x.Date >= from && x.Date < to)
            .SumAsync(x => x.Amount, ct);
    }

    private async Task<IReadOnlyList<BudgetDto>> EnrichWithSpentAsync(IReadOnlyList<Budget> entities, CancellationToken ct)
    {
        var result = new List<BudgetDto>(entities.Count);
        foreach (var e in entities)
        {
            var spent = await ComputeSpentAsync(e.UserProfileId, e.CategoryId, e.Month, ct);
            result.Add(MapToDto(e, spent));
        }
        return result;
    }

    private static bool TryParseMonth(string month, out int year, out int mon)
    {
        year = mon = 0;
        var parts = month.Split('-');
        return parts.Length == 2 && int.TryParse(parts[0], out year) && int.TryParse(parts[1], out mon);
    }

    private static BudgetDto MapToDto(Budget e, decimal spent) => new(
        e.Id, e.UserProfileId, e.CategoryId,
        e.Category?.Name ?? string.Empty,
        e.Month, e.Amount, spent, e.Amount - spent,
        e.CreatedAt, e.UpdatedAt);

    public async Task<IReadOnlyList<BudgetRealizationDto>> GetRealizationsAsync(Guid userProfileId, string month, CancellationToken ct = default)
    {
        var entities = await _repo.GetByMonthAsync(userProfileId, month, ct);
        var result = new List<BudgetRealizationDto>(entities.Count);
        foreach (var e in entities)
        {
            var spent = await ComputeSpentAsync(e.UserProfileId, e.CategoryId, e.Month, ct);
            result.Add(MapToRealizationDto(e, spent));
        }
        return result;
    }

    private static BudgetRealizationDto MapToRealizationDto(Budget e, decimal spent)
    {
        var percentage = e.Amount == 0 ? 0 : Math.Round((double)spent / (double)e.Amount * 100, 2);
        return new BudgetRealizationDto(
            e.Id, e.UserProfileId, e.CategoryId,
            e.Category?.Name ?? string.Empty,
            e.Category?.IconOrColor ?? string.Empty,
            e.Month, e.Amount, spent, e.Amount - spent, percentage,
            e.CreatedAt, e.UpdatedAt);
    }
}
