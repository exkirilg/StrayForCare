using Services.Tags.Dto;

namespace Services.Tags;

public interface ITagsServices : IServicesErrors
{
    Task<IEnumerable<TagDto>> GetTagsWithPagination(GetTagsRequest request);
    Task<TagDto?> GetTagByIdAsync(ushort tagId);
    Task<ushort> NewTagAsync(NewTagRequest request);
    Task UpdateTagNameAsync(UpdateTagNameRequest request);
    Task SoftDeleteAsync(ushort tagId);
    Task DeleteAsync(ushort tagId);
}
