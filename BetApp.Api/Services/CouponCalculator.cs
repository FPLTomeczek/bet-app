namespace BetApp.Api.Services;

/// <summary>
/// The financial maths of a coupon, and nothing else: no database, no HTTP, no clock.
/// Kept separate from <see cref="CouponService"/> precisely because these rules are the
/// part worth testing in isolation — they are deterministic, and getting the rounding
/// wrong costs real money.
/// </summary>
public static class CouponCalculator
{
    /// <summary>
    /// Accumulator odds: the selection odds multiplied together. Rounded to 3 decimals
    /// to match the precision the odds are stored with (numeric(8,3)).
    /// </summary>
    /// <exception cref="ArgumentException">A coupon with no selections has no odds — that is
    /// a caller bug, not a 1.0 accumulator, so it must not be silently rounded into a
    /// payout equal to the stake.</exception>
    public static decimal CalculateTotalOdds(IReadOnlyCollection<decimal> selectionOdds)
    {
        if (selectionOdds.Count == 0)
            throw new ArgumentException("A coupon must have at least one selection.", nameof(selectionOdds));

        var product = selectionOdds.Aggregate(1m, (accumulated, odds) => accumulated * odds); 

        return Math.Round(product, 3, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// What the coupon pays if every selection wins: stake x total odds, rounded to 2
    /// decimals (numeric(12,2)). Takes the already-rounded total odds, so the figure the
    /// punter is shown is the one the payout is derived from.
    /// </summary>
    public static decimal CalculatePotentialPayout(decimal stake, decimal totalOdds) =>
        Math.Round(stake * totalOdds, 2, MidpointRounding.AwayFromZero);
}
