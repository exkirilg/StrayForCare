using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Services.Runners;
using Services.Tags.DbAccess;
using Services.Tags.Dto;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services.Tags;

public class TagsServices
{
    private readonly RunnerWriteDbAsync<NewTagRequest, Tag> _runner;
    private readonly List<ValidationResult> _errors = new();

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();
    public bool HasErrors => _errors.Any();

    public TagsServices(DataContext context)
    {
        _runner = new RunnerWriteDbAsync<NewTagRequest, Tag>(
            context,
            new NewTagAction(new TagsDbAccess(context))
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns><see cref="Tag.TagId"/></returns>
    public async Task<ushort> NewTagAsync(NewTagRequest request)
    {
        Tag? tag = null;

        try
        {
            tag = await _runner.RunActionAsync(request);
            if (_runner.HasErrors) _errors.AddRange(_runner.Errors);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException postgresEx)
                throw;

            switch (postgresEx.ConstraintName)
            {
                case "IX_Tags_Name":
                    _errors.Add(
                        new ValidationResult(
                            "Name is not unique",
                            new string[] { nameof(Tag.Name) }));
                    break;
                default:
                    throw;
            }
        }
        
        if (_runner.HasErrors) return default;

        return tag is not null ? tag.TagId : default;
    }
}
