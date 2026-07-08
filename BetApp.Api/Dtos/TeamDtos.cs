using System.ComponentModel.DataAnnotations;

namespace BetApp.Api.Dtos;

public record TeamResponse(int Id, string Name, int SportCategoryId, string? Country, string? ImageUrl);

public record CreateTeamRequest(
    [Required][MaxLength(150)] string Name,
    [Range(1, int.MaxValue)] int SportCategoryId,
    [MaxLength(60)] string? Country,
    string? ImageUrl);

public record UpdateTeamRequest(
    [Required][MaxLength(150)] string Name,
    [Range(1, int.MaxValue)] int SportCategoryId,
    [MaxLength(60)] string? Country,
    string? ImageUrl);
