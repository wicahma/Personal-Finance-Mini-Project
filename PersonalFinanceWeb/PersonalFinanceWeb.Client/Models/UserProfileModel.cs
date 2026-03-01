namespace PersonalFinanceWeb.Client.Models;

public sealed class UserProfileModel
{
    public Guid Id { get; init; }
    public string IdentityUserId { get; init; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class UpdateProfileRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = string.Empty;
}
