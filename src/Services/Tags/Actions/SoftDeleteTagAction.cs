using Domain.Models;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class SoftDeleteTagAction : ActionErrors, IActionAsync<Guid, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public SoftDeleteTagAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(Guid id)
    {
        Tag tag = await _dbAccess.GetTagByIdAsync(id);

        if (tag.SoftDeleted)
        {
            SaveChangesIsNotNeeded = true;
            return tag;
        }

        tag.SoftDeleted = true;

        return tag;
    }
}
