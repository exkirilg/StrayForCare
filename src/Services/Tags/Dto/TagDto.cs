using Domain.Models;

namespace Services.Tags.Dto;

public record TagDto(
    Guid Id,
    string Name,
    bool SoftDeleted
)
{
    public static TagDto FromTag(Tag tag)
    {
        return new TagDto(tag.Id, tag.Name, tag.SoftDeleted);
    }
}
