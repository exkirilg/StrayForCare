using Domain.Models;
using Services.Issues.DbAccess;
using Services.Issues.Dto;

namespace Services.Issues.Actions;

public class AddTagToIssueAction : ActionErrors, IActionAsync<AddTagToIssueRequest, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public AddTagToIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(AddTagToIssueRequest dto)
    {
        Issue issue = await _dbAccess.GetIssueByIdAsync(dto.IssueId);
        Tag tag = await _dbAccess.GetTagByIdAsync(dto.TagId);

        issue.AddTag(tag);

        return issue;
    }
}
