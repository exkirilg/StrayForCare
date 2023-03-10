namespace Services.Issues.Dto;

public record AddTagToIssueRequest(
    Guid IssueId,
    Guid TagId
);
