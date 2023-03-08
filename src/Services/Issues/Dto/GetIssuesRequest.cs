using System.ComponentModel.DataAnnotations;

namespace Services.Issues.Dto;

public record GetIssuesRequest
    : IValidatableObject
{
    public int PageSize { get; set; } = 10;

    public int PageNum { get; set; } = 1;

    public string SortBy { get; set; } = nameof(GetIssuesRequestSortByOptions.Distance);

    public bool Descending { get; set; } = false;

    public double CurrentLocationLatitude { get; set; } = 0;

    public double CurrentLocationLongitude { get; set; } = 0;

    public int InDistance { get; set; } = 50000;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PageSize <= 0)
        {
            yield return new ValidationResult(
                "Page size must be greater than 0", new string[] { nameof(PageSize) }
            );
        }

        if (PageSize > 100)
        {
            yield return new ValidationResult(
                "Page size must not be greater than 100", new string[] { nameof(PageSize) }
            );
        }

        if (PageNum <= 0)
        {
            yield return new ValidationResult(
                "Page number must be greater than 0", new string[] { nameof(PageNum) }
            );
        }
    }
}

public enum GetIssuesRequestSortByOptions
{
    CreatedAt,
    Distance
}
