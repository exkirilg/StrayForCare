using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services.Runners;

public class RunnerReadDbAsync<TIn, TOut>
{
    private readonly IActionAsync<TIn, TOut> _actionClass;
    private readonly List<ValidationResult> _errors = new();

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();
    public bool HasErrors => _errors.Any();

    public RunnerReadDbAsync(IActionAsync<TIn, TOut> actionClass)
    {
        _actionClass = actionClass;
    }

    public async Task<TOut> RunActionAsync(TIn dataIn)
    {
        var result = await _actionClass.ActionAsync(dataIn).ConfigureAwait(false);

        if (_actionClass.HasErrors) _errors.AddRange(_actionClass.Errors);

        return result;
    }
}
