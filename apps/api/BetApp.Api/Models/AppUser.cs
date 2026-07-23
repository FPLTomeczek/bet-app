using System.Text.Json.Serialization;

namespace BetApp.Api.Models;

public class AppUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public decimal Balance { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime CreatedAt { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}

// String-serialized in JSON and OpenAPI via [JsonConverter] on the type.
[JsonConverter(typeof(JsonStringEnumConverter<UserStatus>))]
public enum UserStatus { Active, Suspended, Closed }
