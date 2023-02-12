namespace Services.Tags.Dto;

public record GetTagsRequest
(
    int PageSize,
    int PageStartZeroBased,
    string? NameSearch,
    bool? Descending
);
