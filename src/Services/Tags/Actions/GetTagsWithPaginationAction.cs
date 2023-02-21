using Services.Tags.DbAccess;
using Services.Tags.Dto;

namespace Services.Tags.Actions;

public class GetTagsWithPaginationAction : ActionErrors, IActionAsync<GetTagsRequest, IEnumerable<TagDto>>
{
    private readonly ITagsDbAccess _dbAccess;

    public GetTagsWithPaginationAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<IEnumerable<TagDto>> ActionAsync(GetTagsRequest request)
    {
        return await _dbAccess.GetTagsDtoWithPaginationAsync(request);
    }
}
