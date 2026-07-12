using System.ComponentModel.DataAnnotations;
using BetApp.Api.Models;

namespace BetApp.Api.Dtos;

public record BonusResponse(int Id, string Name, BonusType Type, decimal Value, DateOnly? ValidFrom, DateOnly? ValidTo);

public record CreateBonusRequest(
    [Required][MaxLength(120)] string Name,
    BonusType Type,
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true)] decimal Value,
    DateOnly? ValidFrom,
    DateOnly? ValidTo);

public record UpdateBonusRequest(
    [Required][MaxLength(120)] string Name,
    BonusType Type,
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true)] decimal Value,
    DateOnly? ValidFrom,
    DateOnly? ValidTo);
