using Domain.Models;
using Services.Dto;

namespace Services.Tags.Dto;

public record TagDto : BaseEntityDto
{
    public string Name { get; init; }

    public TagDto(Guid id, bool softDeleted, string name) : base(id, softDeleted)
    {
        Name = name;
    }

    public static TagDto FromTag(Tag tag)
    {
        return new TagDto(tag.Id, tag.SoftDeleted, tag.Name);
    }
}
