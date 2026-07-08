using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(60);

        // ImageUrl maps to TEXT (no length limit).

        builder.HasOne(x => x.SportCategory)
            .WithMany()
            .HasForeignKey(x => x.SportCategoryId)
            // A category cannot be removed while teams still reference it.
            .OnDelete(DeleteBehavior.Restrict);
    }
}
