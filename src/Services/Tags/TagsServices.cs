using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Services.Exceptions;
using Services.Runners;
using Services.Tags.Actions;
using Services.Tags.DbAccess;
using Services.Tags.Dto;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Services.Tags;

public class TagsServices : ITagsServices
{
    private readonly DataContext _context;
    private readonly List<ValidationResult> _errors = new();

    public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();
    public bool HasErrors => _errors.Any();

    public TagsServices(DataContext context)
    {
        _context = context;
    }

    public void ClearErrors()
    {
        _errors.Clear();
    }

    public async Task<IEnumerable<TagDto>> GetTagsWithPagination(GetTagsRequest request)
    {
        RunnerReadDbAsync<GetTagsRequest, IEnumerable<TagDto>> runner = new(
            new GetTagsWithPaginationAction(new TagsDbAccess(_context))
        );

        IEnumerable<TagDto> result = Enumerable.Empty<TagDto>();

        try
        {
            result = await runner.RunActionAsync(request);
            if (runner.HasErrors) _errors.AddRange(runner.Errors);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            if (ex.ParamName is null ||
                !(ex.ParamName != nameof(GetTagsRequest.PageSize) 
                || ex.ParamName != nameof(GetTagsRequest.PageStartZeroBased)))
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

    public async Task<TagDto?> GetTagByIdAsync(ushort tagId)
    {
        RunnerReadDbAsync<ushort, Tag> runner = new(
            new GetTagByIdAction(new TagsDbAccess(_context))
        );

        Tag? tag = null;

        try
        {
            tag = await runner.RunActionAsync(tagId);
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

        return TagDto.FromTag(tag);
    }

    public async Task<ushort> NewTagAsync(NewTagRequest request)
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

        return tag is not null ? tag.TagId : default;
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

    public async Task SoftDeleteAsync(ushort tagId)
    {
        RunnerWriteDbAsync<ushort, Tag> runner = new(
            _context,
            new SoftDeleteTagAction(new TagsDbAccess(_context))
        );

        try
        {
            _ = await runner.RunActionAsync(tagId);
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

    public async Task DeleteAsync(ushort tagId)
    {
        RunnerWriteDbAsync<ushort, Tag> runner = new(
            _context,
            new DeleteTagAction(new TagsDbAccess(_context))
        );

        try
        {
            _ = await runner.RunActionAsync(tagId);
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
