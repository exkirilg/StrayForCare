using DataAccess;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services.Runners;

public class RunnerWriteDbAsync<TIn, TOut>
{
    private readonly DataContext _context;
    private readonly IActionAsync<TIn, TOut> _actionClass;
    private readonly List<ValidationResult> _errors = new();

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();
    public bool HasErrors => _errors.Any();

    public RunnerWriteDbAsync(DataContext context, IActionAsync<TIn, TOut> actionClass)
    {
        _context = context;
        _actionClass = actionClass;
    }

    public async Task<TOut> RunActionAsync(TIn dataIn)
    {
        var result = await _actionClass.ActionAsync(dataIn).ConfigureAwait(false);

        if (_actionClass.HasErrors) _errors.AddRange(_actionClass.Errors);

        if (!HasErrors && !_actionClass.SaveChangesIsNotNeeded)
        {
            var errors = await _context.SaveChangesWithValidationAsync();
            if (errors.Any()) _errors.AddRange(errors);
        }

        return result;
    }
}
