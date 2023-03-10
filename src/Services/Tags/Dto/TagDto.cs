using Domain.Models;
using Services.Dto;

namespace Services.Tags.Dto;

public record TagDto : BaseEntityDto
{
    public string Name { get; init; }

    public TagDto(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public TagDto(Tag tag)
        : this(tag.Id, tag.Name)
    {
    }
}
