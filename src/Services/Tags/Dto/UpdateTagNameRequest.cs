namespace Services.Tags.Dto;

public record UpdateTagNameRequest(
    ushort TagId,
    string Name
);
