using Domain.Models;
using Services.Tags.DbAccess;

namespace Services.Tags.Actions;

public class GetTagByIdAction : ActionErrors, IActionAsync<Guid, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public GetTagByIdAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(Guid id)
    {
        return await _dbAccess.GetTagByIdAsync(id);
    }
}
