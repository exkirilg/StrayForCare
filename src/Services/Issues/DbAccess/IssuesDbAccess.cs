using DataAccess;
using Domain.Helpers;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Services.Exceptions;
using Services.Issues.Dto;
using Services.Issues.Queries;
using Services.Queries;
using System.Linq.Expressions;

namespace Services.Issues.DbAccess;

public class IssuesDbAccess : IIssuesDbAccess
{
    private readonly DataContext _context;

    public IssuesDbAccess(DataContext context)
    {
        _context = context;
    }

    public async Task<GetIssuesResponse> GetIssuesDtoWithPaginationAsync(GetIssuesRequest request)
    {
        Point currentLocation = LocationHelper.CreateLocationByCoordinates(request.CurrentLocationLatitude, request.CurrentLocationLongitude);

        Expression<Func<Issue, bool>>? filter = null;
        if (request.InDistance != default)
        {
            filter = IssuesFilter.FilterByDistanceExpression(request.InDistance, currentLocation);
        }

        IssuesOrderByOptions orderBy = IssuesOrderByOptions.ByDistanceAscending;
        if (request.SortBy == nameof(GetIssuesRequestSortByOptions.CreatedAt))
        {
            orderBy = request.Descending ?
                IssuesOrderByOptions.ByCreatedAtDescending : IssuesOrderByOptions.ByCreatedAtAscending;
        }
        else if (request.SortBy == nameof(GetIssuesRequestSortByOptions.Distance))
        {
            orderBy = request.Descending ?
                IssuesOrderByOptions.ByDistanceDescending : IssuesOrderByOptions.ByDistanceAscending;
        }

        return new GetIssuesResponse(
            await _context.Issues
                .AsNoTracking()
                .FilterIssues(filter)
                .OrderIssues(orderBy, currentLocation)
                .MapIssuesToDto(currentLocation)
                .Page(request.PageSize, request.PageNum)
                .ToListAsync(),
            await _context.Issues.CountAsync()
        );
    }

    public async Task<Issue> GetIssueByIdAsync(Guid id)
    {
        Issue? result = await _context.Issues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(issue => issue.Id == id);

        if (result is null)
            throw new NoEntityFoundByIdException($"There is no {nameof(Issue)} with id {id}", nameof(Issue.Id));

        return result;
    }

    public async Task AddAsync(Issue newIssue)
    {
        await _context.AddAsync(newIssue);
    }

    public void Remove(Issue issue)
    {
        _context.Remove(issue);
    }
}
