using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services.Tags.Queries;

public static class TagsFilter
{
    public static IQueryable<Tag> FilterTags(this IQueryable<Tag> tags, Expression<Func<Tag, bool>>? filter = null)
    {
        if (filter is null) return tags;

        return tags
            .Where(filter);
    }

    public static Expression<Func<Tag, bool>> FilterByNameExpression(string nameSearch)
    {
        return tag => EF.Functions.ILike(tag.Name, $"%{nameSearch}%");
    }
}
