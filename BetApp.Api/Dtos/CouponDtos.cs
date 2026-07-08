using System.ComponentModel.DataAnnotations;
using BetApp.Api.Models;

namespace BetApp.Api.Dtos;

public record CouponSelectionResponse(int Id, int EventId, string Market, string Selection, decimal Odds, SelectionResult Result);

public record CouponResponse(
    int Id,
    int UserId,
    int? BonusId,
    decimal Stake,
    decimal TotalOdds,
    decimal PotentialPayout,
    CouponStatus Status,
    DateTime PlacedAt,
    IReadOnlyList<CouponSelectionResponse> Selections);

// A coupon is placed atomically with its selections (the coupon is the aggregate root).
// TotalOdds and PotentialPayout are computed server-side, not supplied by the client.
public record CreateCouponSelectionRequest(
    [Range(1, int.MaxValue)] int EventId,
    [Required][MaxLength(50)] string Market,
    [Required][MaxLength(50)] string Selection,
    [Range(typeof(decimal), "1.0", "9999.999", ParseLimitsInInvariantCulture = true)] decimal Odds);

public record CreateCouponRequest(
    [Range(1, int.MaxValue)] int UserId,
    int? BonusId,
    [Range(typeof(decimal), "0.01", "999999999999", ParseLimitsInInvariantCulture = true)] decimal Stake,
    [Required][MinLength(1)] List<CreateCouponSelectionRequest> Selections);
