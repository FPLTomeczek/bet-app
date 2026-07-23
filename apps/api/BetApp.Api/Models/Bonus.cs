using System.Text.Json.Serialization;

namespace BetApp.Api.Models;

public class Bonus
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public BonusType Type { get; set; }
    public decimal Value { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }

    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}

// String-serialized in JSON and OpenAPI via [JsonConverter] on the type.
[JsonConverter(typeof(JsonStringEnumConverter<BonusType>))]
public enum BonusType { FreeBet, DepositMatch, OddsBoost }
