using Services.Tags.Dto;

namespace Services.Tags;

public interface ITagsServices : IServicesErrors
{
    Task<IEnumerable<TagDto>> GetTagsWithPagination(GetTagsRequest request);
    Task<TagDto?> GetTagByIdAsync(Guid id);
    Task<Guid> NewTagAsync(NewTagRequest request);
    Task UpdateTagNameAsync(UpdateTagNameRequest request);
    Task SoftDeleteAsync(Guid id);
    Task DeleteAsync(Guid id);
}
