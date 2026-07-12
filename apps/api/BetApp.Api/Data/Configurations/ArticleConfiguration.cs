using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetApp.Api.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        // Content maps to TEXT.
        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(120);

        builder.Property(x => x.PublishedAt)
            .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.SportCategory)
            .WithMany()
            .HasForeignKey(x => x.SportCategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
