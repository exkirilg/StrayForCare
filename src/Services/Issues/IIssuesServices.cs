using Services.Issues.Dto;

namespace Services.Issues; 

public interface IIssuesServices : IServicesErrors
{
    Task<GetIssuesResponse?> GetIssuesWithPagination(GetIssuesRequest request);
    Task<IssueDto?> GetIssueByIdAsync(Guid id);
    Task<Guid> NewIssueAsync(NewIssueRequest request);
    Task UpdateIssueAsync(UpdateIssueRequest request);
    Task SoftDeleteIssueAsync(Guid id);
    Task DeleteIssueAsync(Guid id);
}
