namespace Services.Issues.Dto;

public record RemoveTagFromIssueRequest(
    Guid IssueId,
    Guid TagId
);
