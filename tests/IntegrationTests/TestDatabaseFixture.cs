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

                    SeedIssuesTestData(context);
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

    private void SeedIssuesTestData(DataContext context)
    {
        List<Issue> issues = new();

        Issue issue;

        issue = new Issue()
        {
            Title = "Very cute stray kitten for adoption",
            Description = "I've just found an adorable kitten on the street, it would be so nice if someone would take care of it!"
        };
        issue.SetLocation(33.525392803570526, 44.616615916982816);
        issues.Add(issue);

        Task.Delay(10).Wait();

        issue = new Issue()
        {
            Title = "A band of adorable puppies",
            Description = "Great bunch of cutest puppies in the world!"
        };
        issue.SetLocation(25.935264334932786, 48.29284943040044);
        issues.Add(issue);

        Task.Delay(10).Wait();

        issue = new Issue()
        {
            Title = "Tortoise Manifique",
            Description = "Just a turtle."
        };
        issue.SetLocation(48.86281344205555, 2.2597164566729004);
        issues.Add(issue);

        Task.Delay(10).Wait();

        issue = new Issue()
        {
            Title = "I'm simple bat",
            Description = "and you are dead"
        };
        issue.SetLocation(30.718260692320914, 114.46375925631736);
        issues.Add(issue);

        Task.Delay(10).Wait();

        issue = new Issue()
        {
            Title = "Alligators doesn't bite!",
            Description = "Were his last words."
        };
        issue.SetLocation(30.395633523176564, -91.69046257720453);
        issues.Add(issue);

        context.AddRange(issues);
    }

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
