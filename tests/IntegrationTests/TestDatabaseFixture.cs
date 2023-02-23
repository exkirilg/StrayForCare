using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public class TestDatabaseFixture
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    private readonly IConfiguration _configuration;

    public TestDatabaseFixture()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"appsettings.json", false, false)
            .AddUserSecrets<TestDatabaseFixture>()
            .AddEnvironmentVariables()
            .Build();

        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    SeedTagsTestData(context);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public DataContext CreateContext()
        => new(
            new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(
                    _configuration.GetConnectionString("AppDataConnection"),
                    x => x.UseNetTopologySuite())
                .Options);

    private void SeedTagsTestData(DataContext context)
    {
        context.AddRange(new Tag[]
        {
            new Tag() { Name = "Cat" },
            new Tag() { Name = "Dog" },
            new Tag() { Name = "Starving" },
            new Tag() { Name = "Injured" },
            new Tag() { Name = "Sick" },
            new Tag() { Name = "Adoption" },
            new Tag() { Name = "Aggressive" }
        });
    }
}
