using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfigurations;

public class TagsConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .Property(tag => tag.Id);

        builder
            .Property(tag => tag.Name)
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(20);

        builder
            .HasIndex(tag => tag.Name)
            .IsUnique();

        builder
            .HasQueryFilter(tag => !tag.SoftDeleted);
    }
}
