using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Services;

public interface ITransactionService
{
    Task<TransactionDetailDto?> GetDetailByIdAsync(Guid id, CancellationToken ct = default);
    Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedList<TransactionDto>> GetPagedAsync(Guid userProfileId, TransactionQueryDto query, CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetByMonthAsync(Guid userProfileId, int year, int month, CancellationToken ct = default);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken ct = default);
    Task<(TransactionDto From, TransactionDto To)> CreateTransferAsync(CreateTransferDto dto, CancellationToken ct = default);
    Task<TransactionDto> UpdateAsync(UpdateTransactionDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
