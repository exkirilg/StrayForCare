using Domain;
using Services.Tags.DbAccess;
using Services.Tags.Dto;

namespace Services.Tags;

public class NewTagAction : ActionErrors, IActionAsync<NewTagRequest, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public NewTagAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(NewTagRequest dto)
    {
        Tag newTag = new(dto.Name);

        await _dbAccess.AddAsync(newTag);

        return newTag;
    }
}
