using Domain.Models;
using Services.Issues.DbAccess;
using Services.Issues.Dto;

namespace Services.Issues.Actions;

public class NewIssueAction : ActionErrors, IActionAsync<NewIssueRequest, Issue>
{
    private readonly IIssuesDbAccess _dbAccess;

    public NewIssueAction(IIssuesDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Issue> ActionAsync(NewIssueRequest dto)
    {
        Issue newIssue = new()
        {
            Title = dto.Title,
            Description = dto.Description
        };
        newIssue.SetLocation(dto.Latitude, dto.Longitude);

        await _dbAccess.AddAsync(newIssue);

        return newIssue;
    }
}
