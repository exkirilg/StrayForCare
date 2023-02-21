using Domain.Models;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class DeleteTagAction : ActionErrors, IActionAsync<Guid, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public DeleteTagAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(Guid id)
    {
        Tag tag = await _dbAccess.GetTagByIdAsync(id);

        _dbAccess.Remove(tag);

        return tag;
    }
}
