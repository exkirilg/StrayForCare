using Services.Issues.Dto;

namespace Services.Issues; 

public interface IIssuesServices : IServicesErrors
{
    Task<IssueDto?> GetIssueByIdAsync(Guid id);
    Task<Guid> NewIssueAsync(NewIssueRequest request);
    Task UpdateIssueAsync(UpdateIssueRequest request);
}
