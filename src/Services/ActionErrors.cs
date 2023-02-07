using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services;

/// <summary>
/// Provides error handling
/// </summary>
public abstract class ActionErrors
{
    private readonly List<ValidationResult> _errors = new();

    public bool SaveChangesIsNotNeeded { get; protected set; }

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();

    public bool HasErrors => _errors.Any();

    protected void AddError(string errorMessage, params string[] propertiesNames)
    {
        _errors.Add(new ValidationResult(errorMessage, propertiesNames));
    }
}
