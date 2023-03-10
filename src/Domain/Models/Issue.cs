using Domain.Helpers;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Issue : BaseEntity, IValidatableObject
{
    private readonly DateTime _createdAt = DateTime.UtcNow;

    public DateTime CreatedAt => _createdAt;

    private Point _location = LocationHelper.DefaultLocation;

    public Point Location => _location;

    public void SetLocation(double latitude, double longitude)
    {
        _location = LocationHelper.CreateLocationByCoordinates(latitude, longitude);
    }

    private string _title = string.Empty;
    private string _description = string.Empty;

    public string Title
    {
        get => _title;
        set => _title = value.Trim();
    }

    public string Description
    {
        get => _description;
        set => _description = value.Trim();
    }

    private readonly List<Tag> _tags = new();

    public IReadOnlyList<Tag> Tags => _tags;

    public void AddTag(Tag tag)
    {
        if (_tags.Contains(tag)) return;
        _tags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        _tags.Remove(tag);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult(
                "Title must be filled", new string[] { nameof(Title) }
            );
        }

        if (Title.Length > 250)
        {
            yield return new ValidationResult(
                $"Title length must not exceed {250} characters", new string[] { nameof(Title) }
            );
        }

        if (Description.Length > 2500)
        {
            yield return new ValidationResult(
                $"Description length must not exceed {2500} characters", new string[] { nameof(Description) }
            );
        }
    }
}
