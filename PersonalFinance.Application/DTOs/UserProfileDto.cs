namespace PersonalFinance.Application.DTOs;

public record UserProfileDto(
    Guid Id,
    string IdentityUserId,
    string DisplayName,
    string DefaultCurrency,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record UpdateProfileDto(
    string DisplayName,
    string DefaultCurrency);
