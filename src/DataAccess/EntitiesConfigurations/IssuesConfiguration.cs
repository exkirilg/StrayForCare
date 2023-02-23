using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfigurations;

internal class IssuesConfiguration : BaseEntityConfiguration<Issue>
{
    public override void Configure(EntityTypeBuilder<Issue> builder)
    {
        base.Configure(builder);

        builder
            .Property(issue => issue.Title)
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(issue => issue.Location)
            .IsRequired()
            .HasColumnType("geography");

        builder
            .Property(issue => issue.Description)
            .HasColumnType("text")
            .HasMaxLength(2500);
    }
}
