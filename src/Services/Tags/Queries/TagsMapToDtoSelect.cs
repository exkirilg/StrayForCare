using Domain;
using Services.Tags.Dto;

namespace Services.Tags.Queries;

public static class TagsMapToDtoSelect
{
    public static IQueryable<TagDto> MapTagsToDto(this IQueryable<Tag> tags)
    {
        return tags.Select(tag => new TagDto(
            tag.TagId,
            tag.Name,
            tag.SoftDeleted
        ));
    }
}
