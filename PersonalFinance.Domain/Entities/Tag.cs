using PersonalFinance.Domain.Abstractions;

namespace PersonalFinance.Domain.Entities;

public class Tag : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#FFFFFF";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}
