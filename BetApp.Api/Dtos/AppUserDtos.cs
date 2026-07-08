using System.ComponentModel.DataAnnotations;
using BetApp.Api.Models;

namespace BetApp.Api.Dtos;

// PasswordHash is intentionally absent — hashes must never leave the API.
// Balance and Status are server-controlled (balance changes only via transactions).
public record AppUserResponse(int Id, string Username, string Email, decimal Balance, UserStatus Status, DateTime CreatedAt);

public record CreateAppUserRequest(
    [Required][MaxLength(50)] string Username,
    [Required][EmailAddress][MaxLength(255)] string Email,
    [Required][MinLength(8)] string Password);

public record UpdateAppUserRequest(
    [Required][MaxLength(50)] string Username,
    [Required][EmailAddress][MaxLength(255)] string Email);
