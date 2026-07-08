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
