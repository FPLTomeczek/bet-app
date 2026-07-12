using BetApp.Api.Services;

namespace BetApp.Api.Tests.Services;

public class CouponCalculatorTests
{
    [Fact]
    public void CalculateTotalOdds_SingleSelection_ReturnsThatSelectionsOdds()
    {
        var totalOdds = CouponCalculator.CalculateTotalOdds([2.50m]);

        Assert.Equal(2.50m, totalOdds);
    }

    [Fact]
    public void CalculateTotalOdds_MultipleSelections_MultipliesThemTogether()
    {
        var totalOdds = CouponCalculator.CalculateTotalOdds([2.00m, 1.50m, 3.00m]);

        Assert.Equal(9.00m, totalOdds);
    }

    [Fact]
    public void CalculateTotalOdds_ProductHasMoreThanThreeDecimals_RoundsToThree()
    {
        // 1.333 * 1.333 * 1.333 = 2.368593037
        var totalOdds = CouponCalculator.CalculateTotalOdds([1.333m, 1.333m, 1.333m]);

        Assert.Equal(2.369m, totalOdds);
    }

    [Fact]
    public void CalculateTotalOdds_ProductLandsExactlyOnAHalf_RoundsUpNotToEven()
    {
        // 1.05 * 1.05 = 1.1025 — exactly halfway at the third decimal. Banker's rounding
        // (the .NET default) would round to even and give 1.102; the house rounds away
        // from zero, so this must be 1.103.
        var totalOdds = CouponCalculator.CalculateTotalOdds([1.05m, 1.05m]);

        Assert.Equal(1.103m, totalOdds);
    }

    [Fact]
    public void CalculateTotalOdds_NoSelections_Throws()
    {
        // An empty accumulator would silently produce odds of 1.0 — i.e. a coupon that
        // pays back exactly the stake. That is never a valid coupon, so it must blow up.
        var exception = Assert.Throws<ArgumentException>(
            () => CouponCalculator.CalculateTotalOdds([]));

        Assert.Equal("selectionOdds", exception.ParamName);
    }

    [Fact]
    public void CalculatePotentialPayout_MultipliesStakeByOdds()
    {
        var payout = CouponCalculator.CalculatePotentialPayout(stake: 100.00m, totalOdds: 3.000m);

        Assert.Equal(300.00m, payout);
    }

    [Fact]
    public void CalculatePotentialPayout_ResultLandsExactlyOnAHalfPenny_RoundsUpNotToEven()
    {
        // 10.10 * 1.25 = 12.625 — halfway at the second decimal. Banker's rounding would
        // give 12.62; away-from-zero gives 12.63.
        var payout = CouponCalculator.CalculatePotentialPayout(stake: 10.10m, totalOdds: 1.25m);

        Assert.Equal(12.63m, payout);
    }

    [Fact]
    public void PlacingACoupon_DerivesThePayoutFromTheRoundedOdds_NotTheRawProduct()
    {
        // Guards the order of operations: odds are rounded first (2.368593037 -> 2.369),
        // and the payout follows from that displayed figure. Paying out on the raw product
        // would give 236.86 and quietly contradict the odds shown to the punter.
        var totalOdds = CouponCalculator.CalculateTotalOdds([1.333m, 1.333m, 1.333m]);

        var payout = CouponCalculator.CalculatePotentialPayout(stake: 100.00m, totalOdds: totalOdds);

        Assert.Equal(236.90m, payout);
    }
}
