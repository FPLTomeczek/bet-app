using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class CouponSelectionConfiguration : IEntityTypeConfiguration<CouponSelection>
{
    public void Configure(EntityTypeBuilder<CouponSelection> builder)
    {
        builder.Property(x => x.Market)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Selection)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Odds)
            .HasPrecision(8, 3)
            .IsRequired();

        builder.Property(x => x.Result)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Coupon)
            .WithMany(c => c.Selections)
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            // Keep settled selections even if someone tries to remove the event.
            .OnDelete(DeleteBehavior.Restrict);
    }
}
