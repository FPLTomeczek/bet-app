using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BetApp.Api.Data;

public class BetAppContext : DbContext
{
    public BetAppContext(DbContextOptions<BetAppContext> options) : base(options) { }

    public DbSet<SportCategory> SportCategories => Set<SportCategory>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Bonus> Bonuses => Set<Bonus>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponSelection> CouponSelections => Set<CouponSelection>();
    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Force singular, snake_case table names to match the ER diagram
        // (sport_category, event_participant, ...). By default EF derives the
        // table name from the DbSet property (plural). We must produce the final
        // snake_case name ourselves: the snake_case naming convention only rewrites
        // names it inferred, and skips ones set explicitly here — so setting the
        // PascalCase CLR name would leave the table PascalCase while columns stay
        // snake_case. Emitting snake_case directly keeps everything consistent.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.DisplayName()));
        }
    }

    private static string ToSnakeCase(string name) =>
        string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c)
                ? "_" + char.ToLowerInvariant(c)
                : char.ToLowerInvariant(c).ToString()));
}
