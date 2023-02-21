namespace Domain.Models;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public bool SoftDeleted { get; set; }
}
