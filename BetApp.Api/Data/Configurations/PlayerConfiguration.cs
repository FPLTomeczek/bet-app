using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Nationality)
            .HasMaxLength(3);

        builder.Property(x => x.Position)
            .HasMaxLength(40);

        builder.HasOne(x => x.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(x => x.TeamId)
            // Deleting a team detaches its players rather than removing them.
            .OnDelete(DeleteBehavior.SetNull);
    }
}
