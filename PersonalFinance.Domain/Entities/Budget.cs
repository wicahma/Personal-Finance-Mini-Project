using PersonalFinance.Domain.Abstractions;

namespace PersonalFinance.Domain.Entities;

public class Budget : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserProfileId { get; set; }
    public Guid CategoryId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
