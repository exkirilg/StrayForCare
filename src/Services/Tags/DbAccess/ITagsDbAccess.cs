using Domain;

namespace Services.Tags.DbAccess;

public interface ITagsDbAccess
{
    Task<Tag> GetTagById(ushort TagId);
    Task AddAsync(Tag newTag);
}
