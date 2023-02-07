using Domain;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class SoftDeleteTagAction : ActionErrors, IActionAsync<ushort, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public SoftDeleteTagAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(ushort tagId)
    {
        Tag tag = await _dbAccess.GetTagById(tagId);

        if (tag.SoftDeleted)
        {
            SaveChangesIsNotNeeded = true;
            return tag;
        }

        tag.SoftDeleted = true;

        return tag;
    }
}
