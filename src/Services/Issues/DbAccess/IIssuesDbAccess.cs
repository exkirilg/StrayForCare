using Domain.Models;

namespace Services.Issues.DbAccess;

public interface IIssuesDbAccess
{
    Task<Issue> GetIssueByIdAsync(Guid id);
    Task AddAsync(Issue newIssue);
    void Remove(Issue issue);
}
