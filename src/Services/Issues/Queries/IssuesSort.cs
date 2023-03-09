using Domain.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Services.Issues.Queries;

public static class IssuesSort
{
    public static IQueryable<Issue> OrderIssues(this IQueryable<Issue> issues, IssuesOrderByOptions orderByOption, Point currentLocation)
    {
        return orderByOption switch
        {
            IssuesOrderByOptions.ByCreatedAtAscending => issues.OrderBy(issue => issue.CreatedAt),
            IssuesOrderByOptions.ByCreatedAtDescending => issues.OrderByDescending(issue => issue.CreatedAt),
            IssuesOrderByOptions.ByDistanceAscending => issues.OrderBy(issue => EF.Functions.DistanceKnn(issue.Location, currentLocation)),
            IssuesOrderByOptions.ByDistanceDescending => issues.OrderByDescending(issue => EF.Functions.DistanceKnn(issue.Location, currentLocation)),
            _ => throw new ArgumentOutOfRangeException(nameof(orderByOption), orderByOption, null),
        };
    }
}

public enum IssuesOrderByOptions
{
    ByCreatedAtAscending,
    ByCreatedAtDescending,
    ByDistanceAscending,
    ByDistanceDescending
}
