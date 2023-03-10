using Domain.Models;
using Services.Tags.DbAccess;
using Services.Tags.Dto;

namespace Services.Tags.Actions;

public class UpdateTagNameAction : ActionErrors, IActionAsync<UpdateTagNameRequest, Tag>
{
    private readonly ITagsDbAccess _dbAccess;

    public UpdateTagNameAction(ITagsDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Tag> ActionAsync(UpdateTagNameRequest dto)
    {
        Tag tag = await _dbAccess.GetTagByIdAsync(dto.Id);

        tag.Name = dto.Name;

        return tag;
    }
}
