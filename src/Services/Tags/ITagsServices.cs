using Services.Tags.Dto;

namespace Services.Tags;

public interface ITagsServices : IServicesErrors
{
    Task<TagDto?> GetTagByIdAsync(ushort tagId);
    Task<ushort> NewTagAsync(NewTagRequest request);
    Task UpdateTagNameAsync(UpdateTagNameRequest request);
    Task SoftDeleteAsync(ushort tagId);
    Task DeleteAsync(ushort tagId);
}
