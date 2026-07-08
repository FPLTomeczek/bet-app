namespace BetApp.Api.Models;

public class Coupon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? BonusId { get; set; }
    public decimal Stake { get; set; }
    public decimal TotalOdds { get; set; }
    public decimal PotentialPayout { get; set; }
    public CouponStatus Status { get; set; } = CouponStatus.Placed;
    public DateTime PlacedAt { get; set; }

    public AppUser? User { get; set; }
    public Bonus? Bonus { get; set; }
    public ICollection<CouponSelection> Selections { get; set; } = new List<CouponSelection>();
}
