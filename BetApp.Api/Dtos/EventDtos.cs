using System.ComponentModel.DataAnnotations;
using BetApp.Api.Models;

namespace BetApp.Api.Dtos;

public record EventResponse(int Id, int SportCategoryId, string Name, DateTime StartTime, EventStatus Status, string? Result);

public record CreateEventRequest(
    [Range(1, int.MaxValue)] int SportCategoryId,
    [Required][MaxLength(200)] string Name,
    DateTime StartTime,
    EventStatus Status = EventStatus.Scheduled,
    [MaxLength(50)] string? Result = null);

public record UpdateEventRequest(
    [Range(1, int.MaxValue)] int SportCategoryId,
    [Required][MaxLength(200)] string Name,
    DateTime StartTime,
    EventStatus Status,
    [MaxLength(50)] string? Result);

// event_participant is managed as a sub-resource of event (no standalone controller).
public record EventParticipantResponse(int Id, int EventId, int? TeamId, int? PlayerId, ParticipantSide? Side);

public record CreateEventParticipantRequest(int? TeamId, int? PlayerId, ParticipantSide? Side);
