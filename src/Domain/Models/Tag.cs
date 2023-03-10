using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Tag : BaseEntity, IComparable<Tag>, IValidatableObject
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = value.Trim();
    }

    public int CompareTo(Tag? other)
    {
        return other is null ? -1 : Name.CompareTo(other.Name);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult(
                "Name must be filled", new string[] { nameof(Name) }
            );
        }

        if (Name.Length > 20)
        {
            yield return new ValidationResult(
                "Name length must not exceed 20 characters", new string[] { nameof(Name) }
            );
        }
    }
}
