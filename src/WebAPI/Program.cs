using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services.Tags;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0.0", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "Stray For Care",
        Contact = new OpenApiContact
        {
            Email = builder.Configuration["Contacts:Email"]
        },
        License = new OpenApiLicense
        {
            Name = "MIT Licence"
        }
    });

    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AppDataConnection"),
        x => x.UseNetTopologySuite()
    );
});

builder.Services.AddScoped<ITagsServices, TagsServices>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1.0.0/swagger.json", "Stray For Care API v1.0.0");
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
