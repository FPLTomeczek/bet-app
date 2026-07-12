using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class SportCategoryConfiguration : IEntityTypeConfiguration<SportCategory>
{
    public void Configure(EntityTypeBuilder<SportCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}