using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AppDataConnection"),
        x => x.UseNetTopologySuite()
    );
});

var app = builder.Build();

app.MapGet("/", () => string.Empty);

app.Run();
