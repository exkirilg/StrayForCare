using System.ComponentModel.DataAnnotations;

namespace Services.Tags.Dto;

public record GetTagsRequest
(
    int PageSize = 10,
    int PageNum = 1,
    string NameSearch = "",
    bool Descending = false
)
    : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PageSize <= 0)
        {
            yield return new ValidationResult(
                "Page size must be greater than 0", new string[] { nameof(PageSize) }
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
