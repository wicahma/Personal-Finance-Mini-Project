using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class Transaction : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserProfileId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CategoryId { get; set; }

    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public bool IsTransfer { get; set; }
    public Guid? TransferPairId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Category? Category { get; set; }
    public Transaction? TransferPair { get; set; }
    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}
