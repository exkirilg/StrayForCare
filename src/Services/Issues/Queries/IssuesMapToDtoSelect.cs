using Domain.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Services.Issues.Dto;

namespace Services.Issues.Queries;

public static class IssuesMapToDtoSelect
{
    public static IQueryable<IssueDto> MapIssuesToDto(this IQueryable<Issue> issues, Point currentLocation)
    {
        return issues.Select(issue => new IssueDto(
            issue,
            EF.Functions.DistanceKnn(issue.Location, currentLocation))
        );
    }
}
