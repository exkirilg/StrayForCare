namespace Services.Issues.Dto;

public record NewIssueRequest(
    string Title,
    double Latitude,
    double Longitude,
    string Description
);
