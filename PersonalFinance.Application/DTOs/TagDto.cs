namespace PersonalFinance.Application.DTOs;

public record TagDto(
    Guid Id,
    Guid UserProfileId,
    string Name,
    string Color,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateTagDto(
    Guid UserProfileId,
    string Name,
    string Color = "#FFFFFF");

public record UpdateTagDto(
    Guid Id,
    string Name,
    string Color);
