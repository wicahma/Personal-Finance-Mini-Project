using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public record CategoryDto(
    Guid Id,
    Guid UserProfileId,
    string Name,
    CategoryType Type,
    string IconOrColor,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateCategoryDto(
    Guid UserProfileId,
    string Name,
    CategoryType Type,
    string IconOrColor = "");

public record UpdateCategoryDto(
    Guid Id,
    string Name,
    CategoryType Type,
    string IconOrColor);
