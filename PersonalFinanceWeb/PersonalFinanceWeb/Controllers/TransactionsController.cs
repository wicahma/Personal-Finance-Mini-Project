using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Services;
using PersonalFinance.Domain.Enums;

namespace PersonalFinanceWeb.Controllers;

[Authorize]
public sealed class TransactionsController : BaseApiController
{
    private readonly ITransactionService _transactionService;
    private readonly IUserProfileService _profileService;

    public TransactionsController(ITransactionService transactionService, IUserProfileService profileService)
    {
        _transactionService = transactionService;
        _profileService = profileService;
    }

    // GET /api/transactions?startDate=&endDate=&accountId=&categoryId=&tagId=&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? accountId,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var query = new TransactionQueryDto(startDate, endDate, accountId, categoryId, tagId, page, pageSize);
        var paged = await _transactionService.GetPagedAsync(profileId, query, ct);

        var pagination = new PaginationMeta
        {
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize,
            TotalItems = paged.TotalItems,
            TotalPages = paged.TotalPages
        };
        return OkData(paged.Items, "Success", pagination);
    }

    // GET /api/transactions/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransaction(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var transaction = await _transactionService.GetDetailByIdAsync(id, ct);
        if (transaction is null || transaction.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Transaction not found."));
        return OkData(transaction);
    }

    // POST /api/transactions
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateTransactionDto(
            profileId, req.AccountId, req.CategoryId,
            req.Amount, req.Date, req.Notes ?? string.Empty,
            req.Type, req.TagIds);
        var tx = await _transactionService.CreateAsync(dto, ct);
        return CreatedData(nameof(GetTransaction), new { id = tx.Id }, tx, "Transaction created successfully.");
    }

    // POST /api/transactions/transfer
    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var dto = new CreateTransferDto(
            profileId, req.SourceAccountId, req.DestinationAccountId,
            req.Amount, req.Date, req.Notes ?? string.Empty);
        var (fromTx, toTx) = await _transactionService.CreateTransferAsync(dto, ct);
        return OkData(new List<TransactionDto> { fromTx, toTx }, "Transfer created successfully.");
    }

    // PUT /api/transactions/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionRequest req, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _transactionService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Transaction not found."));

        var dto = new UpdateTransactionDto(
            id, req.CategoryId, req.Amount, req.Date,
            req.Notes ?? string.Empty, req.Type ?? existing.Type, req.TagIds);
        var tx = await _transactionService.UpdateAsync(dto, ct);
        return OkData(tx, "Transaction updated successfully.");
    }

    // DELETE /api/transactions/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id, CancellationToken ct)
    {
        var profileId = await GetUserProfileIdAsync(_profileService, ct);
        var existing = await _transactionService.GetByIdAsync(id, ct);
        if (existing is null || existing.UserProfileId != profileId)
            return NotFound(ApiResponse<object>.Failure("Transaction not found."));

        await _transactionService.DeleteAsync(id, ct);
        return NoContentResponse();
    }
}

public record CreateTransactionRequest(
    Guid AccountId,
    Guid? CategoryId,
    decimal Amount,
    DateTime Date,
    TransactionType Type,
    string? Notes = null,
    IList<Guid>? TagIds = null);

public record CreateTransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    DateTime Date,
    string? Notes = null);

public record UpdateTransactionRequest(
    decimal Amount,
    DateTime Date,
    Guid? CategoryId = null,
    TransactionType? Type = null,
    string? Notes = null,
    IList<Guid>? TagIds = null);
