using Services.Tags.DbAccess;
using Services.Tags.Dto;

namespace Services.Tags.Actions;

public class GetTagsWithPaginationAction : ActionErrors, IActionAsync<GetTagsRequest, GetTagsResponse>
{
    private readonly ITagsDbAccess _dbAccess;

    public GetTagsWithPaginationAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<GetTagsResponse> ActionAsync(GetTagsRequest request)
    {
        return await _dbAccess.GetTagsDtoWithPaginationAsync(request);
    }
}
