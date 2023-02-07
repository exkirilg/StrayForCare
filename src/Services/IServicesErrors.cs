using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services;

public interface IServicesErrors
{
    IImmutableList<ValidationResult> Errors { get; }
    bool HasErrors { get; }
    void ClearErrors();
}
