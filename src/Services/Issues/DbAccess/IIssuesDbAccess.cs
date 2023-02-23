using Domain.Models;

namespace Services.Issues.DbAccess;

public interface IIssuesDbAccess
{
    Task<Issue> GetIssueByIdAsync(Guid id);
}
