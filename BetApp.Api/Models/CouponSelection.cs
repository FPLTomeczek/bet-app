namespace BetApp.Api.Models;

public class CouponSelection
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public int EventId { get; set; }
    public required string Market { get; set; }
    public required string Selection { get; set; }
    public decimal Odds { get; set; }
    public SelectionResult Result { get; set; } = SelectionResult.Open;

    public Coupon? Coupon { get; set; }
    public Event? Event { get; set; }
}
