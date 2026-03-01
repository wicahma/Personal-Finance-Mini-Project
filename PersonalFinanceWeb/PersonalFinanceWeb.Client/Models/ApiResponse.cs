namespace PersonalFinanceWeb.Client.Models;

public sealed class PaginationMeta
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}

public sealed class ApiResponse<T>
{
    public bool Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public PaginationMeta? Pagination { get; init; }
    public string? Error { get; init; }
}
