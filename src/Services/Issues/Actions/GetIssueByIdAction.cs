using Domain.Models;
using Services.Issues.DbAccess;

namespace Services.Issues.Actions;

public class GetIssueByIdAction : ActionErrors, IActionAsync<Guid, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public GetIssueByIdAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(Guid id)
    {
        return await _dbAccess.GetIssueByIdAsync(id);
    }
}
