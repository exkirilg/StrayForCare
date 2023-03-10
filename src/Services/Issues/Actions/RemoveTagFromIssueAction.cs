using Domain.Models;
using Services.Issues.DbAccess;
using Services.Issues.Dto;

namespace Services.Issues.Actions;

public class RemoveTagFromIssueAction : ActionErrors, IActionAsync<RemoveTagFromIssueRequest, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public RemoveTagFromIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(RemoveTagFromIssueRequest dto)
    {
        Issue issue = await _dbAccess.GetIssueByIdAsync(dto.IssueId);
        Tag tag = await _dbAccess.GetTagByIdAsync(dto.TagId);

        issue.RemoveTag(tag);

        return issue;
    }
}
