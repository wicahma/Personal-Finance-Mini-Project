using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface ICategoryClientService
{
    Task<IReadOnlyList<CategoryManagementModel>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CategoryManagementModel>> GetByTypeAsync(string type, CancellationToken ct = default);
    Task<CategoryManagementModel?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<CategoryManagementModel?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default);
}
