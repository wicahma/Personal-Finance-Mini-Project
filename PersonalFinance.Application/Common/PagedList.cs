namespace PersonalFinance.Application.Common;

public sealed class PagedList<T>
{
    public List<T> Items { get; init; } = new();
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
