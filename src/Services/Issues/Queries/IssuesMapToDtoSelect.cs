using Domain.Models;
using Services.Issues.Dto;

namespace Services.Issues.Queries;

public static class IssuesMapToDtoSelect
{
    public static IQueryable<IssueDto> MapIssuesToDto(this IQueryable<Issue> issues)
    {
        return issues.Select(issue => new IssueDto(issue));
    }
}
