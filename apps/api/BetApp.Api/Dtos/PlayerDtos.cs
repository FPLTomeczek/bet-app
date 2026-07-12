using System.ComponentModel.DataAnnotations;

namespace BetApp.Api.Dtos;

public record PlayerResponse(int Id, string Name, int? TeamId, string? Nationality, string? Position, DateOnly? BirthDate);

public record CreatePlayerRequest(
    [Required][MaxLength(150)] string Name,
    int? TeamId,
    [MaxLength(3)] string? Nationality,
    [MaxLength(40)] string? Position,
    DateOnly? BirthDate);

public record UpdatePlayerRequest(
    [Required][MaxLength(150)] string Name,
    int? TeamId,
    [MaxLength(3)] string? Nationality,
    [MaxLength(40)] string? Position,
    DateOnly? BirthDate);
