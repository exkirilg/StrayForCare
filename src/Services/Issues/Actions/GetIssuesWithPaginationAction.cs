using Services.Issues.DbAccess;
using Services.Issues.Dto;

namespace Services.Issues.Actions;

public class GetIssuesWithPaginationAction : ActionErrors, IActionAsync<GetIssuesRequest, GetIssuesResponse>
{
    private readonly IIssuesDbAccess _dbAccess;

    public GetIssuesWithPaginationAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<GetIssuesResponse> ActionAsync(GetIssuesRequest request)
    {
        return await _dbAccess.GetIssuesDtoWithPaginationAsync(request);
    }
}
