using Services.Tags.Dto;

namespace Services.Tags;

public interface ITagsServices : IServicesErrors
{
    Task<GetTagsResponse?> GetTagsWithPagination(GetTagsRequest request);
    Task<TagDto?> GetTagByIdAsync(Guid id);
    Task<Guid> NewTagAsync(NewTagRequest request);
    Task UpdateTagNameAsync(UpdateTagNameRequest request);
    Task SoftDeleteTagAsync(Guid id);
    Task DeleteTagAsync(Guid id);
}
