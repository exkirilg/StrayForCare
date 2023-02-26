namespace Services.Tags.Dto;

public record GetTagsResponse
(
    IEnumerable<TagDto> Tags,
    int TotalCount
);
