namespace PersonalFinance.Domain.Entities;

public class TransactionTag
{
    public Guid TransactionId { get; set; }
    public Guid TagId { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
