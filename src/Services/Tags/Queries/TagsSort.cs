using Domain;

namespace Services.Tags.Queries;

public static class TagsSort
{
    public static IQueryable<Tag> OrderTags(this IQueryable<Tag> tags, TagsOrderByOptions orderByOption)
    {
        return orderByOption switch
        {
            TagsOrderByOptions.ByNameAscending => tags.OrderBy(tag => tag.Name),
            TagsOrderByOptions.ByNameDescending => tags.OrderByDescending(tag => tag.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(orderByOption), orderByOption, null),
        };
    }
}

public enum TagsOrderByOptions
{
    ByNameAscending,
    ByNameDescending
}
