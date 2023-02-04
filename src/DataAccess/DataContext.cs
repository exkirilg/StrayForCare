using DataAccess.EntitiesConfigurations;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

	public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder
			.HasPostgresExtension("postgis")
			.ApplyConfiguration(new TagsConfiguration());
    }
}
