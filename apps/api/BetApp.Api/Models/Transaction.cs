using System.Text.Json.Serialization;

namespace BetApp.Api.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? Method { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser? User { get; set; }
}

// String-serialized in JSON and OpenAPI via [JsonConverter] on the type.
[JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
public enum TransactionType { Deposit, Withdrawal }

[JsonConverter(typeof(JsonStringEnumConverter<TransactionStatus>))]
public enum TransactionStatus { Pending, Completed, Failed }
