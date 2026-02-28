namespace PersonalFinance.Application.Common;

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

    public static ApiResponse<T> Success(T data, string message = "Success", PaginationMeta? pagination = null) =>
        new() { Status = true, Message = message, Data = data, Pagination = pagination };

    public static ApiResponse<T> Failure(string message, string? error = null) =>
        new() { Status = false, Message = message, Error = error };
}
