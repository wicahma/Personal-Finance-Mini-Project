using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Domain.Abstractions;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Tag>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<Tag> AddAsync(Tag tag, CancellationToken ct = default);
    Task UpdateAsync(Tag tag, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
