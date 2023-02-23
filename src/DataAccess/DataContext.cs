using DataAccess.EntitiesConfigurations;
using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace DataAccess;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

    public DbSet<Issue> Issues { get; set; } = null!;
	public DbSet<Tag> Tags { get; set; } = null!;

    public async Task<IImmutableList<ValidationResult>> SaveChangesWithValidationAsync()
    {
        var result = ExecuteValidation();

        if (result.Any()) return result;

        await SaveChangesAsync();

        return result;
    }

    private IImmutableList<ValidationResult> ExecuteValidation()
    {
        List<ValidationResult> result = new();
        
        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            ValidationContext valContext = new(entry.Entity);
            List<ValidationResult> entityErrors = new();
            if (!Validator.TryValidateObject(entry.Entity, valContext, entityErrors, true))
            {
                result.AddRange(entityErrors);
            }
        }

        return result.ToImmutableList();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder
			.HasPostgresExtension("postgis")
            .ApplyConfiguration(new IssuesConfiguration())
			.ApplyConfiguration(new TagsConfiguration());
    }
}
