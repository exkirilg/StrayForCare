using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Exceptions;
using Services.Queries;
using Services.Tags.Dto;
using Services.Tags.Queries;
using System.Linq.Expressions;

namespace Services.Tags.DbAccess;

public class TagsDbAccess : ITagsDbAccess
{
    private readonly DataContext _context;

    public TagsDbAccess(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagDto>> GetTagsDtoWithPaginationAsync(GetTagsRequest request)
    {
        Expression<Func<Tag, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.NameSearch))
            filter = TagsFilter.FilterByNameExpression(request.NameSearch);

        TagsOrderByOptions orderBy = TagsOrderByOptions.ByNameAscending;
        if (request.Descending)
            orderBy = TagsOrderByOptions.ByNameDescending;

        return await _context.Tags
            .AsNoTracking()
            .FilterTags(filter)
            .OrderTags(orderBy)
            .MapTagsToDto()
            .Page(request.PageSize, request.PageNum)
            .ToListAsync();
    }

    public async Task<Tag> GetTagByIdAsync(Guid id)
    {
        Tag? result = await _context.Tags
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(tag => tag.Id == id);

        if (result is null)
            throw new NoEntityFoundByIdException($"There is no Tag with id {id}", nameof(Tag.Id));

        return result;
    }

    public async Task AddAsync(Tag newTag)
    {
        await _context.AddAsync(newTag);
    }

    public void Remove(Tag tag)
    {
        _context.Remove(tag);
    }
}
