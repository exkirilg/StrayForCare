using Domain.Models;
using NetTopologySuite.Geometries;
using Services.Dto;
using Services.Tags.Dto;

namespace Services.Issues.Dto;

public record IssueDto : BaseEntityDto
{
    public DateTime CreatedAt { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Distance { get; init; }
    public List<TagDto> Tags { get; init; } = new();

    public IssueDto(
        Guid id, DateTime createdAt, string title,
        string description, Point location,
        double distance, IEnumerable<TagDto> tags
    ) : base(id)
    {
        CreatedAt = createdAt;
        Title = title;
        Description = description;
        Latitude = location.Y;
        Longitude = location.X;
        Distance = distance;
        Tags.AddRange(tags);
    }

    public IssueDto(Issue issue, double distance)
        : this(issue.Id, issue.CreatedAt, issue.Title,
              issue.Description, issue.Location, distance,
              issue.Tags.Select(tag => new TagDto(tag)))
    {
    }

    public IssueDto(Issue issue)
        : this(issue, 0)
    {
    }
}
