using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Result)
            .HasMaxLength(50);

        builder.HasIndex(x => x.StartTime);

        builder.HasOne(x => x.SportCategory)
            .WithMany()
            .HasForeignKey(x => x.SportCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
