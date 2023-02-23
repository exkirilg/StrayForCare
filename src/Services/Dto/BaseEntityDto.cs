namespace Services.Dto;

public abstract record BaseEntityDto
{
    public Guid Id { get; init; }
    public bool SoftDeleted { get; init; }

    public BaseEntityDto(Guid id, bool softDeleted)
    {
        Id = id;
        SoftDeleted = softDeleted;
    }
}
