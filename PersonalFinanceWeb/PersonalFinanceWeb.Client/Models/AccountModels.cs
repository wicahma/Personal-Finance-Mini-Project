namespace PersonalFinanceWeb.Client.Models;

public sealed class CreateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Bank";
    public decimal InitialBalance { get; set; }
}

public sealed class UpdateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}
