using Domain;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class GetTagByIdAction : ActionErrors, IActionAsync<ushort, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public GetTagByIdAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(ushort tagId)
    {
        return await _dbAccess.GetTagByIdAsync(tagId);
    }
}
