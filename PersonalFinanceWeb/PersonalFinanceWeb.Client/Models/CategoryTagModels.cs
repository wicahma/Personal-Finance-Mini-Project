namespace PersonalFinanceWeb.Client.Models;


public sealed class CategoryManagementModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string IconOrColor { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class TagManagementModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#FFFFFF";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}


public sealed class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Expense";
    public string IconOrColor { get; set; } = "#FFFFFF";
}

public sealed class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? IconOrColor { get; set; }
}

public sealed class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#FFFFFF";
}

public sealed class UpdateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
