using Domain;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class DeleteTagAction : ActionErrors, IActionAsync<ushort, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public DeleteTagAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(ushort tagId)
    {
        Tag tag = await _dbAccess.GetTagById(tagId);

        _dbAccess.Remove(tag);

        return tag;
    }
}
