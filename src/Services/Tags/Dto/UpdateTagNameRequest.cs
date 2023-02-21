namespace Services.Tags.Dto;

public record UpdateTagNameRequest(
    Guid Id,
    string Name
);
