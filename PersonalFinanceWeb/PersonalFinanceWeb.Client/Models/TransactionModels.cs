namespace PersonalFinanceWeb.Client.Models;

public sealed class TransactionListItem
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string AccountName { get; init; } = string.Empty;
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Notes { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsTransfer { get; init; }
    public Guid? TransferPairId { get; init; }
    public List<TagItem> Tags { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class TagItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#FFFFFF";
}

public sealed class TransactionDetailModel
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string AccountName { get; init; } = string.Empty;
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public string? CategoryIconOrColor { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Notes { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsTransfer { get; init; }
    public Guid? TransferPairId { get; init; }
    public List<TagItem> Tags { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class TransactionFilterModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? TagId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class CreateTransactionRequest
{
    public Guid AccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Type { get; set; } = "Expense";
    public string Notes { get; set; } = string.Empty;
    public List<Guid> TagIds { get; set; } = [];
}

public sealed class CreateTransferRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Notes { get; set; } = string.Empty;
}

public sealed class UpdateTransactionRequest
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public List<Guid>? TagIds { get; set; }
}
