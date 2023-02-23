using DataAccess;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services;

public abstract class ServicesErrors
{
    protected readonly DataContext _context;
    protected readonly List<ValidationResult> _errors = new();

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();
    public bool HasErrors => _errors.Any();

    public ServicesErrors(DataContext context)
    {
        _context = context;
    }

    public void ClearErrors()
    {
        _errors.Clear();
    }
}
