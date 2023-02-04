using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfigurations;

public class TagsConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .Property(tag => tag.TagId)
            .HasColumnType("smallint");

        builder
            .Property(tag => tag.Name)
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(20);
    }
}
