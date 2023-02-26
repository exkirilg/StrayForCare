using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfigurations;

internal class IssuesConfiguration : BaseEntityConfiguration<Issue>
{
    public override void Configure(EntityTypeBuilder<Issue> builder)
    {
        base.Configure(builder);
        
        builder.Property(issue => issue.CreatedAt)
            .HasField("_createdAt")
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder
            .Property(issue => issue.Title)
            .HasField("_title")
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(issue => issue.Location)
            .HasField("_location")
            .IsRequired()
            .HasColumnType("geography");

        builder
            .Property(issue => issue.Description)
            .HasField("_description")
            .HasColumnType("text")
            .HasMaxLength(2500);
    }
}
