using PersonalFinanceWeb.Client.Models;

namespace PersonalFinanceWeb.Client.Services;

public interface ITagClientService
{
    Task<IReadOnlyList<TagManagementModel>> GetTagsAsync(CancellationToken ct = default);
    Task<TagManagementModel?> CreateTagAsync(CreateTagRequest request, CancellationToken ct = default);
    Task<TagManagementModel?> UpdateTagAsync(Guid id, UpdateTagRequest request, CancellationToken ct = default);
    Task<bool> DeleteTagAsync(Guid id, CancellationToken ct = default);
}
