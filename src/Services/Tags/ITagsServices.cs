using Services.Tags.Dto;

namespace Services.Tags;

public interface ITagsServices : IServicesErrors
{
    Task<ushort> NewTagAsync(NewTagRequest request);
    Task UpdateTagNameAsync(UpdateTagNameRequest request);
}
