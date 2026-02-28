namespace PersonalFinanceWeb.Client.Models;

public sealed class ApiResponse<T>
{
    public bool Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public string? Error { get; init; }
}
