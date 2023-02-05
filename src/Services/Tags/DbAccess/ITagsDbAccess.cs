using Domain;

namespace Services.Tags.DbAccess;

public interface ITagsDbAccess
{
    Task AddAsync(Tag newTag);
}
