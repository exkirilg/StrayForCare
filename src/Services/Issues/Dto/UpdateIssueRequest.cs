namespace Services.Issues.Dto;

public record UpdateIssueRequest(
    Guid Id,
    string Title,
    double Latitude,
    double Longitude,
    string Description
);
