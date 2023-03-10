using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfigurations;

public class TagsConfiguration : BaseEntityConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder
            .Property(tag => tag.Name)
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(20);

        builder
            .HasIndex(tag => tag.Name)
            .IsUnique();

        builder
            .HasMany<Issue>("_issues")
            .WithMany("_tags")
            .UsingEntity("IssueTag");

        builder
            .Ignore(tag => tag.Issues);
    }
}
