using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface ITransactionClientService
{
    Task<(List<TransactionListItem> Items, PaginationMeta? Pagination)> GetPagedAsync(
        TransactionFilterModel filter, CancellationToken ct = default);

    Task<TransactionDetailModel?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<TransactionListItem?> CreateAsync(CreateTransactionRequest request, CancellationToken ct = default);

    Task<bool> CreateTransferAsync(CreateTransferRequest request, CancellationToken ct = default);

    Task<TransactionListItem?> UpdateAsync(Guid id, UpdateTransactionRequest request, CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
