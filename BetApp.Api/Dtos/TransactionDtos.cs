using System.ComponentModel.DataAnnotations;
using BetApp.Api.Models;

namespace BetApp.Api.Dtos;

public record TransactionResponse(int Id, int UserId, TransactionType Type, decimal Amount, TransactionStatus Status, string? Method, DateTime CreatedAt);

// Status is server-controlled (starts Pending). Amount must be positive.
public record CreateTransactionRequest(
    [Range(1, int.MaxValue)] int UserId,
    TransactionType Type,
    [Range(typeof(decimal), "0.01", "999999999999", ParseLimitsInInvariantCulture = true)] decimal Amount,
    [MaxLength(40)] string? Method);
