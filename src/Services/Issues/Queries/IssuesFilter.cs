using Domain.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Services.Issues.Queries;

public static class IssuesFilter
{
    public static IQueryable<Issue> FilterIssues(this IQueryable<Issue> issues, Expression<Func<Issue, bool>>? filter = null)
    {
        if (filter is null) return issues;

        return issues
            .Where(filter);
    }

    public static Expression<Func<Issue, bool>> FilterByDistanceExpression(int distance, Point currentLocation)
    {
        return issue => EF.Functions.IsWithinDistance(issue.Location, currentLocation, distance, true);
    }
}
