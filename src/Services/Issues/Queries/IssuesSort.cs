using Domain.Models;

namespace Services.Issues.Queries;

public static class IssuesSort
{
    public static IQueryable<Issue> OrderIssues(this IQueryable<Issue> issues, IssuesOrderByOptions orderByOption)
    {
        return orderByOption switch
        {
            IssuesOrderByOptions.ByCreatedAtAscending => issues.OrderBy(issue => issue.CreatedAt),
            IssuesOrderByOptions.ByCreatedAtDescending => issues.OrderByDescending(issue => issue.CreatedAt),
            IssuesOrderByOptions.ByDistanceAscending => throw new NotImplementedException(),
            IssuesOrderByOptions.ByDistanceDescending => throw new NotImplementedException(),
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
