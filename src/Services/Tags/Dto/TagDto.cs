using Domain;

namespace Services.Tags.Dto;

public record TagDto(
    ushort TagId,
    string Name,
    bool SoftDeleted
)
{
    public static TagDto FromTag(Tag tag)
    {
        return new TagDto(tag.TagId, tag.Name, tag.SoftDeleted);
    }
}
