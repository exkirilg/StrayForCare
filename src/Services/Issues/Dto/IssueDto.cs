using Domain.Models;
using NetTopologySuite.Geometries;
using Services.Dto;

namespace Services.Issues.Dto;

public record IssueDto : BaseEntityDto
{
    public DateTime CreatedAt { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; set; }

    public IssueDto(
        Guid id, bool softDeleted,
        DateTime createdAt, string title,
        string description, Point location
    ) : base(id, softDeleted)
    {
        CreatedAt = createdAt;
        Title = title;
        Description = description;
        Latitude = location.X;
        Longitude = location.Y;
    }

    public IssueDto(Issue issue)
        : this(issue.Id, issue.SoftDeleted, issue.CreatedAt,
              issue.Title, issue.Description, issue.Location)
    {
    }
}
