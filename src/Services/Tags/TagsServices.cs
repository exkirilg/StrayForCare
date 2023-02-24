using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Services.Exceptions;
using Services.Runners;
using Services.Tags.Actions;
using Services.Tags.DbAccess;
using Services.Tags.Dto;
using System.ComponentModel.DataAnnotations;

namespace Services.Tags;

public class TagsServices : ServicesErrors, ITagsServices
{
    public TagsServices(DataContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TagDto>> GetTagsWithPagination(GetTagsRequest request)
    {
        IEnumerable<TagDto> result = Enumerable.Empty<TagDto>();

        var validationResults = request.Validate(new ValidationContext(request));
        if (validationResults.Any())
        {
            _errors.AddRange(validationResults);
            return result;
        }

        RunnerReadDbAsync<GetTagsRequest, IEnumerable<TagDto>> runner = new(
            new GetTagsWithPaginationAction(new TagsDbAccess(_context))
        );

        try
        {
            result = await runner.RunActionAsync(request);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            if (ex.ParamName is null ||
                !(ex.ParamName != nameof(GetTagsRequest.PageSize) 
                || ex.ParamName != nameof(GetTagsRequest.PageNum)))
            {
                throw;
            }

            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.ParamName }));
        }

        return result;
    }

    public async Task<TagDto?> GetTagByIdAsync(Guid id)
    {
        RunnerReadDbAsync<Guid, Tag> runner = new(
            new GetTagByIdAction(new TagsDbAccess(_context))
        );

        Tag? tag = null;

        try
        {
            tag = await runner.RunActionAsync(id);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (NoEntityFoundByIdException ex)
        {
            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.PropertyName }));
        }

        if (tag is null)
            return null;

        return new TagDto(tag);
    }

    public async Task<Guid> NewTagAsync(NewTagRequest request)
    {
        RunnerWriteDbAsync<NewTagRequest, Tag> runner = new (
            _context,
            new NewTagAction(new TagsDbAccess(_context))
        );

        Tag? tag = null;

        try
        {
            tag = await runner.RunActionAsync(request);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
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
        
        if (runner.HasErrors) return default;

        return tag is not null ? tag.Id : default;
    }

    public async Task UpdateTagNameAsync(UpdateTagNameRequest request)
    {
        RunnerWriteDbAsync<UpdateTagNameRequest, Tag> runner = new(
            _context,
            new UpdateTagNameAction(new TagsDbAccess(_context))
        );

        try
        {
            _ = await runner.RunActionAsync(request);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (NoEntityFoundByIdException ex)
        {
            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.PropertyName }));
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
    }

    public async Task SoftDeleteTagAsync(Guid id)
    {
        RunnerWriteDbAsync<Guid, Tag> runner = new(
            _context,
            new SoftDeleteTagAction(new TagsDbAccess(_context))
        );

        try
        {
            _ = await runner.RunActionAsync(id);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (NoEntityFoundByIdException ex)
        {
            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.PropertyName }));
        }
    }

    public async Task DeleteTagAsync(Guid id)
    {
        RunnerWriteDbAsync<Guid, Tag> runner = new(
            _context,
            new DeleteTagAction(new TagsDbAccess(_context))
        );

        try
        {
            _ = await runner.RunActionAsync(id);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (NoEntityFoundByIdException ex)
        {
            _errors.Add(
                new ValidationResult(
                    ex.Message,
                    new string[] { ex.PropertyName }));
        }
    }
}
