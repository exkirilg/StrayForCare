using DataAccess;
using Domain;
using Services.Exceptions;

namespace Services.Tags.DbAccess;

public class TagsDbAccess : ITagsDbAccess
{
    private readonly DataContext _context;

    public TagsDbAccess(DataContext context)
    {
        _context = context;
    }

    public async Task<Tag> GetTagById(ushort TagId)
    {
        Tag? result = await _context.Tags.FindAsync(TagId);

        if (result is null)
            throw new NoEntityFoundByIdException($"There is no Tag with id {TagId}", nameof(Tag.TagId));

        return result;
    }

    public async Task AddAsync(Tag newTag)
    {
        await _context.AddAsync(newTag);
    }
}
