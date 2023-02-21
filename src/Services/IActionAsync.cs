using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services;

public interface IActionAsync<in TIn, TOut>
{
    bool SaveChangesIsNotNeeded { get; }
    IImmutableList<ValidationResult> Errors { get; }
    bool HasErrors { get; }
    Task<TOut> ActionAsync(TIn dto);
}
