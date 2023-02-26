namespace Services.Issues.Dto;

public record GetIssuesResponse
(
    IEnumerable<IssueDto> Issues,
    int TotalCount
);
