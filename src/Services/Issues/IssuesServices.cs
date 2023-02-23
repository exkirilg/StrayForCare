using DataAccess;
using Domain.Models;
using Services.Exceptions;
using Services.Issues.Actions;
using Services.Issues.DbAccess;
using Services.Issues.Dto;
using Services.Runners;
using System.ComponentModel.DataAnnotations;

namespace Services.Issues;

public class IssuesServices : ServicesErrors, IIssuesServices
{
    public IssuesServices(DataContext context) : base(context)
    {
    }

    public async Task<IssueDto?> GetIssueByIdAsync(Guid id)
    {
        RunnerReadDbAsync<Guid, Issue> runner = new(
            new GetIssueByIdAction(new IssuesDbAccess(_context))
        );

        Issue? issues = null;

        try
        {
            issues = await runner.RunActionAsync(id);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (NoEntityFoundByIdException ex)
        {
            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.PropertyName }));
        }

        if (issues is null)
            return null;

        return new IssueDto(issues);
    }
}
