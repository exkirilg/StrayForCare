using Domain.Helpers;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Issue : BaseEntity, IValidatableObject
{
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private string _title = string.Empty;
    private Point _location = GeodataHelper.DefaultLocation;
    private string _description = string.Empty;

    public DateTime CreatedAt => _createdAt;

    public string Title
    {
        get => _title;
        set => _title = value.Trim();
    }

    public Point Location => _location;

    public string Description
    {
        get => _description;
        set => _description = value.Trim();
    }

    public void SetLocation(double latitude, double longitude)
    {
        _location = GeodataHelper.CreateLocationByCoordinates(latitude, longitude);
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

        if (string.IsNullOrWhiteSpace(Description))
        {
            yield return new ValidationResult(
                "Description must be filled", new string[] { nameof(Description) }
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
