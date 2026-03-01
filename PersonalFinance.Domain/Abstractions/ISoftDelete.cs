namespace PersonalFinance.Domain.Abstractions;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
