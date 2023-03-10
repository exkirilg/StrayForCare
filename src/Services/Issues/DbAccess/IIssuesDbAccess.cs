using Domain.Models;
using Services.Issues.Dto;

namespace Services.Issues.DbAccess;

public interface IIssuesDbAccess
{
    Task<GetIssuesResponse> GetIssuesDtoWithPaginationAsync(GetIssuesRequest request);
    Task<Issue> GetIssueByIdAsync(Guid id);
    Task<Tag> GetTagByIdAsync(Guid id);
    Task AddAsync(Issue newIssue);
    void Remove(Issue issue);
}
