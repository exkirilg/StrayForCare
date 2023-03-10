namespace Services.Dto;

public abstract record BaseEntityDto
{
    public Guid Id { get; init; }

    public BaseEntityDto(Guid id)
    {
        Id = id;
    }
}
