using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.Property(x => x.Stake)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.TotalOdds)
            .HasPrecision(8, 3)
            .IsRequired();

        builder.Property(x => x.PotentialPayout)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PlacedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Coupons)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Bonus)
            .WithMany(b => b.Coupons)
            .HasForeignKey(x => x.BonusId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
