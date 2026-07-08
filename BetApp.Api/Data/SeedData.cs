using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BetApp.Api.Data;

/// <summary>
/// Development seed data for a small, curated, deterministic dataset.
///
/// Invoked through EF Core's UseSeeding / UseAsyncSeeding hooks (see Program.cs),
/// which EF runs on Migrate(), EnsureCreated() and on `dotnet ef database update`.
/// The graph is built via navigation properties so EF resolves every foreign key
/// and INSERT order itself — no manual PK/FK bookkeeping like HasData would need.
/// </summary>
public static class SeedData
{
    // Idempotency guard: the hook fires on every `database update`, so we only
    // populate an empty database. SportCategory is the natural "is anything here?"
    // probe because every other aggregate ultimately hangs off it.
    public static void Seed(BetAppContext db)
    {
        if (db.SportCategories.Any())
            return;

        db.AddRange(BuildGraph());
        db.SaveChanges();
    }

    public static async Task SeedAsync(BetAppContext db, CancellationToken cancellationToken)
    {
        if (await db.SportCategories.AnyAsync(cancellationToken))
            return;

        await db.AddRangeAsync(BuildGraph(), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    // Placeholder only — mirrors the app_user convention (no real hashing yet).
    // TODO: replace once PasswordHasher/BCrypt is introduced.
    private const string PasswordPlaceholder = "seed-placeholder-not-a-real-hash";

    // Postgres `timestamp with time zone` (via Npgsql) rejects non-UTC DateTimes,
    // so every timestamp below is constructed with DateTimeKind.Utc.
    private static DateTime Utc(int year, int month, int day, int hour = 12, int minute = 0) =>
        new(year, month, day, hour, minute, 0, DateTimeKind.Utc);

    /// <summary>
    /// Builds the full object graph and returns its aggregate roots. Entities not
    /// listed here (players, participants, coupons, selections, transactions) are
    /// reached through navigation properties and get tracked transitively.
    /// </summary>
    private static IEnumerable<object> BuildGraph()
    {
        // --- Sport categories -------------------------------------------------
        var football = new SportCategory { Name = "Football" };
        var basketball = new SportCategory { Name = "Basketball" };
        var tennis = new SportCategory { Name = "Tennis" };

        // --- Teams (football) with a couple of players each -------------------
        var realMadrid = new Team
        {
            Name = "Real Madrid",
            SportCategory = football,
            Country = "Spain",
            Players =
            {
                new Player { Name = "Vinicius Junior", Nationality = "BRA", Position = "Forward", BirthDate = new DateOnly(2000, 7, 12) },
                new Player { Name = "Jude Bellingham", Nationality = "ENG", Position = "Midfielder", BirthDate = new DateOnly(2003, 6, 29) },
            },
        };
        var barcelona = new Team
        {
            Name = "FC Barcelona",
            SportCategory = football,
            Country = "Spain",
            Players =
            {
                new Player { Name = "Robert Lewandowski", Nationality = "POL", Position = "Forward", BirthDate = new DateOnly(1988, 8, 21) },
                new Player { Name = "Lamine Yamal", Nationality = "ESP", Position = "Forward", BirthDate = new DateOnly(2007, 7, 13) },
            },
        };
        var manCity = new Team
        {
            Name = "Manchester City",
            SportCategory = football,
            Country = "England",
            Players =
            {
                new Player { Name = "Erling Haaland", Nationality = "NOR", Position = "Forward", BirthDate = new DateOnly(2000, 7, 21) },
                new Player { Name = "Rodri", Nationality = "ESP", Position = "Midfielder", BirthDate = new DateOnly(1996, 6, 22) },
            },
        };
        var liverpool = new Team
        {
            Name = "Liverpool",
            SportCategory = football,
            Country = "England",
            Players =
            {
                new Player { Name = "Mohamed Salah", Nationality = "EGY", Position = "Forward", BirthDate = new DateOnly(1992, 6, 15) },
                new Player { Name = "Virgil van Dijk", Nationality = "NED", Position = "Defender", BirthDate = new DateOnly(1991, 7, 8) },
            },
        };

        // --- Events (relative to "today" = 2026-07-08) ------------------------
        // Two finished (with results), one live, one scheduled — enough to exercise
        // every EventStatus and both settled/open coupon selections.
        var elClasico = new Event
        {
            SportCategory = football,
            Name = "Real Madrid vs FC Barcelona",
            StartTime = Utc(2026, 6, 20, 20, 0),
            Status = EventStatus.Finished,
            Result = "2-1",
            Participants =
            {
                new EventParticipant { Team = realMadrid, Side = ParticipantSide.Home },
                new EventParticipant { Team = barcelona, Side = ParticipantSide.Away },
            },
        };
        var cityLiverpool = new Event
        {
            SportCategory = football,
            Name = "Manchester City vs Liverpool",
            StartTime = Utc(2026, 6, 28, 17, 30),
            Status = EventStatus.Finished,
            Result = "1-1",
            Participants =
            {
                new EventParticipant { Team = manCity, Side = ParticipantSide.Home },
                new EventParticipant { Team = liverpool, Side = ParticipantSide.Away },
            },
        };
        var barcaCity = new Event
        {
            SportCategory = football,
            Name = "FC Barcelona vs Manchester City",
            StartTime = Utc(2026, 7, 8, 21, 0),
            Status = EventStatus.Live,
            Participants =
            {
                new EventParticipant { Team = barcelona, Side = ParticipantSide.Home },
                new EventParticipant { Team = manCity, Side = ParticipantSide.Away },
            },
        };
        var realLiverpool = new Event
        {
            SportCategory = football,
            Name = "Real Madrid vs Liverpool",
            StartTime = Utc(2026, 7, 15, 20, 0),
            Status = EventStatus.Scheduled,
            Participants =
            {
                new EventParticipant { Team = realMadrid, Side = ParticipantSide.Home },
                new EventParticipant { Team = liverpool, Side = ParticipantSide.Away },
            },
        };

        // --- Bonuses ----------------------------------------------------------
        var freeBet = new Bonus
        {
            Name = "Welcome Free Bet",
            Type = BonusType.FreeBet,
            Value = 20m,
            ValidFrom = new DateOnly(2026, 1, 1),
            ValidTo = new DateOnly(2026, 12, 31),
        };
        var depositMatch = new Bonus
        {
            Name = "100% Deposit Match",
            Type = BonusType.DepositMatch,
            Value = 100m,
            ValidFrom = new DateOnly(2026, 6, 1),
            ValidTo = new DateOnly(2026, 8, 31),
        };
        var oddsBoost = new Bonus
        {
            Name = "Weekend Odds Boost",
            Type = BonusType.OddsBoost,
            Value = 1.10m,
        };

        // --- Users with transactions and coupons ------------------------------
        var alice = new AppUser
        {
            Username = "alice",
            Email = "alice@example.com",
            PasswordHash = PasswordPlaceholder,
            Balance = 250.00m,
            Status = UserStatus.Active,
            CreatedAt = Utc(2026, 5, 10, 9, 0),
            Transactions =
            {
                new Transaction { Type = TransactionType.Deposit, Amount = 300.00m, Status = TransactionStatus.Completed, Method = "card", CreatedAt = Utc(2026, 5, 10, 9, 5) },
                new Transaction { Type = TransactionType.Withdrawal, Amount = 50.00m, Status = TransactionStatus.Completed, Method = "bank_transfer", CreatedAt = Utc(2026, 6, 25, 14, 0) },
            },
        };
        var bob = new AppUser
        {
            Username = "bob",
            Email = "bob@example.com",
            PasswordHash = PasswordPlaceholder,
            Balance = 75.50m,
            Status = UserStatus.Active,
            CreatedAt = Utc(2026, 5, 18, 11, 30),
            Transactions =
            {
                new Transaction { Type = TransactionType.Deposit, Amount = 100.00m, Status = TransactionStatus.Completed, Method = "card", CreatedAt = Utc(2026, 5, 18, 11, 35) },
                new Transaction { Type = TransactionType.Deposit, Amount = 40.00m, Status = TransactionStatus.Pending, Method = "blik", CreatedAt = Utc(2026, 7, 7, 19, 0) },
            },
        };
        var carol = new AppUser
        {
            Username = "carol",
            Email = "carol@example.com",
            PasswordHash = PasswordPlaceholder,
            Balance = 0.00m,
            Status = UserStatus.Suspended,
            CreatedAt = Utc(2026, 4, 2, 8, 0),
        };

        // Alice: a settled winning coupon on El Clasico (single selection).
        var aliceWon = new Coupon
        {
            User = alice,
            Bonus = freeBet,
            Stake = 20.00m,
            TotalOdds = 1.850m,
            PotentialPayout = 37.00m, // stake * total_odds
            Status = CouponStatus.Won,
            PlacedAt = Utc(2026, 6, 20, 18, 45),
            Selections =
            {
                new CouponSelection
                {
                    Event = elClasico,
                    Market = "Match Result",
                    Selection = "Real Madrid",
                    Odds = 1.850m,
                    Result = SelectionResult.Won,
                },
            },
        };

        // Bob: a settled losing double (two selections, odds multiply).
        var bobLost = new Coupon
        {
            User = bob,
            Stake = 15.00m,
            TotalOdds = 4.410m, // 2.100 * 2.100
            PotentialPayout = 66.15m,
            Status = CouponStatus.Lost,
            PlacedAt = Utc(2026, 6, 28, 16, 0),
            Selections =
            {
                new CouponSelection { Event = cityLiverpool, Market = "Match Result", Selection = "Manchester City", Odds = 2.100m, Result = SelectionResult.Lost },
                new CouponSelection { Event = elClasico, Market = "Both Teams To Score", Selection = "No", Odds = 2.100m, Result = SelectionResult.Won },
            },
        };

        // Alice: an open coupon on the live/upcoming games (still Placed).
        var aliceOpen = new Coupon
        {
            User = alice,
            Stake = 30.00m,
            TotalOdds = 5.250m, // 1.750 * 3.000
            PotentialPayout = 157.50m,
            Status = CouponStatus.Placed,
            PlacedAt = Utc(2026, 7, 8, 20, 30),
            Selections =
            {
                new CouponSelection { Event = barcaCity, Market = "Match Result", Selection = "FC Barcelona", Odds = 1.750m, Result = SelectionResult.Open },
                new CouponSelection { Event = realLiverpool, Market = "Match Result", Selection = "Liverpool", Odds = 3.000m, Result = SelectionResult.Open },
            },
        };

        // --- Articles ---------------------------------------------------------
        var article1 = new Article
        {
            Title = "El Clasico preview: what to watch",
            Content = "Real Madrid host FC Barcelona in a decisive fixture. Key battles across midfield will shape the outcome.",
            Author = "Editorial Team",
            SportCategory = football,
            PublishedAt = Utc(2026, 6, 19, 10, 0),
        };
        var article2 = new Article
        {
            Title = "Betting basics: understanding decimal odds",
            Content = "Decimal odds tell you the total return per unit staked. Multiply your stake by the odds to see the potential payout.",
            Author = "Editorial Team",
            PublishedAt = Utc(2026, 6, 1, 8, 0),
        };

        // Return the aggregate roots. Categories/teams/bonuses referenced only via
        // navigations are still tracked, but listing the roots explicitly keeps the
        // set of top-level inserts obvious.
        return new object[]
        {
            football, basketball, tennis,
            realMadrid, barcelona, manCity, liverpool,
            elClasico, cityLiverpool, barcaCity, realLiverpool,
            freeBet, depositMatch, oddsBoost,
            alice, bob, carol,
            aliceWon, bobLost, aliceOpen,
            article1, article2,
        };
    }
}
