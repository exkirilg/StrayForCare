using Domain.Models;
using Services.Issues.DbAccess;

namespace Services.Issues.Actions;

public class SoftDeleteIssueAction : ActionErrors, IActionAsync<Guid, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public SoftDeleteIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(Guid id)
    {
        Issue issue = await _dbAccess.GetIssueByIdAsync(id);

        if (issue.SoftDeleted)
        {
            SaveChangesIsNotNeeded = true;
            return issue;
        }

        issue.SoftDeleted = true;

        return issue;
    }
}
