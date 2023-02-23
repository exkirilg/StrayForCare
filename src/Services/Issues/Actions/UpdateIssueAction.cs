using Domain.Models;
using Services.Issues.DbAccess;
using Services.Issues.Dto;

namespace Services.Issues.Actions;

public class UpdateIssueAction : ActionErrors, IActionAsync<UpdateIssueRequest, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public UpdateIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(UpdateIssueRequest dto)
    {
        Issue issue = await _dbAccess.GetIssueByIdAsync(dto.Id);

        issue.Title = dto.Title;
        issue.SetLocation(dto.Latitude, dto.Longitude);
        issue.Description = dto.Description;

        return issue;
    }
}
