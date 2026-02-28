using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Domain.Abstractions;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetByMonthAsync(Guid userProfileId, int year, int month, CancellationToken ct = default);
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default);
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
