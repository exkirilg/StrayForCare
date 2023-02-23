using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Exceptions;

namespace Services.Issues.DbAccess;

public class IssuesDbAccess : IIssuesDbAccess
{
    private readonly DataContext _context;

    public IssuesDbAccess(DataContext context)
    {
        _context = context;
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
}
