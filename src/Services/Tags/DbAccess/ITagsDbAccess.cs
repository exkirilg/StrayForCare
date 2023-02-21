using Domain.Models;
using Services.Tags.Dto;

namespace Services.Tags.DbAccess;

public interface ITagsDbAccess
{
    Task<IEnumerable<TagDto>> GetTagsDtoWithPaginationAsync(GetTagsRequest request);
    Task<Tag> GetTagByIdAsync(Guid id);
    Task AddAsync(Tag newTag);
    void Remove(Tag tag);
}
