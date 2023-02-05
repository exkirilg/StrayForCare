using DataAccess;
using Domain;

namespace Services.Tags.DbAccess;

public class TagsDbAccess : ITagsDbAccess
{
    private readonly DataContext _context;

    public TagsDbAccess(DataContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tag newTag)
    {
        await _context.AddAsync(newTag);
    }
}
