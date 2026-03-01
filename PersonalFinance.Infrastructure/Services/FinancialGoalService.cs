using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Services;

public sealed class FinancialGoalService : IFinancialGoalService
{
    private readonly IFinancialGoalRepository _repo;

    public FinancialGoalService(IFinancialGoalRepository repo) => _repo = repo;

    public async Task<FinancialGoalDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IReadOnlyList<FinancialGoalDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<FinancialGoalDto> CreateAsync(CreateFinancialGoalDto dto, CancellationToken ct = default)
    {
        var entity = new FinancialGoal
        {
            UserProfileId = dto.UserProfileId,
            Name = dto.Name,
            Description = dto.Description,
            TargetAmount = dto.TargetAmount,
            CurrentAmount = 0m,
            DeadlineDate = DateOnly.Parse(dto.DeadlineDate),
            CreatedBy = dto.UserProfileId.ToString()
        };
        await _repo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<FinancialGoalDto> UpdateAsync(UpdateFinancialGoalDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Financial goal {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.TargetAmount = dto.TargetAmount;
        entity.CurrentAmount = dto.CurrentAmount;
        entity.DeadlineDate = DateOnly.Parse(dto.DeadlineDate);
        await _repo.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static FinancialGoalDto MapToDto(FinancialGoal e) => new(
        e.Id,
        e.UserProfileId,
        e.Name,
        e.Description,
        e.TargetAmount,
        e.CurrentAmount,
        e.DeadlineDate.ToString("yyyy-MM-dd"),
        e.CreatedAt,
        e.UpdatedAt);
}
