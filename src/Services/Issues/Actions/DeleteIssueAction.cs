using Domain.Models;
using Services.Issues.DbAccess;

namespace Services.Issues.Actions;

public class DeleteIssueAction : ActionErrors, IActionAsync<Guid, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public DeleteIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(Guid id)
    {
        Issue issue = await _dbAccess.GetIssueByIdAsync(id);

        _dbAccess.Remove(issue);

        return issue;
    }
}
