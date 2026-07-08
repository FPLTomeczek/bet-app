using BetApp.Api.Data;
using BetApp.Api.Dtos;
using BetApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly BetAppContext _context;

    public EventsController(BetAppContext context)
    {
        _context = context;
    }

    // ---- event CRUD ----

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Select(e => new EventResponse(e.Id, e.SportCategoryId, e.Name, e.StartTime, e.Status, e.Result))
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponse>> GetById(int id)
    {
        var ev = await _context.Events.FindAsync(id);

        if (ev is null)
            return NotFound();

        return Ok(new EventResponse(ev.Id, ev.SportCategoryId, ev.Name, ev.StartTime, ev.Status, ev.Result));
    }

    [HttpPost]
    public async Task<ActionResult<EventResponse>> Create(CreateEventRequest request)
    {
        if (!await _context.SportCategories.AnyAsync(c => c.Id == request.SportCategoryId))
        {
            ModelState.AddModelError(nameof(request.SportCategoryId), "Sport category does not exist.");
            return ValidationProblem(ModelState);
        }

        var ev = new Event
        {
            SportCategoryId = request.SportCategoryId,
            Name = request.Name,
            StartTime = request.StartTime,
            Status = request.Status,
            Result = request.Result
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        var response = new EventResponse(ev.Id, ev.SportCategoryId, ev.Name, ev.StartTime, ev.Status, ev.Result);
        return CreatedAtAction(nameof(GetById), new { id = ev.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEventRequest request)
    {
        var ev = await _context.Events.FindAsync(id);

        if (ev is null)
            return NotFound();

        if (!await _context.SportCategories.AnyAsync(c => c.Id == request.SportCategoryId))
        {
            ModelState.AddModelError(nameof(request.SportCategoryId), "Sport category does not exist.");
            return ValidationProblem(ModelState);
        }

        ev.SportCategoryId = request.SportCategoryId;
        ev.Name = request.Name;
        ev.StartTime = request.StartTime;
        ev.Status = request.Status;
        ev.Result = request.Result;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events.FindAsync(id);

        if (ev is null)
            return NotFound();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ---- participants (sub-resource of an event, no standalone controller) ----

    [HttpGet("{eventId:int}/participants")]
    public async Task<ActionResult<IEnumerable<EventParticipantResponse>>> GetParticipants(int eventId)
    {
        if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            return NotFound();

        var participants = await _context.EventParticipants
            .AsNoTracking()
            .Where(p => p.EventId == eventId)
            .Select(p => new EventParticipantResponse(p.Id, p.EventId, p.TeamId, p.PlayerId, p.Side))
            .ToListAsync();

        return Ok(participants);
    }

    [HttpGet("{eventId:int}/participants/{participantId:int}")]
    public async Task<ActionResult<EventParticipantResponse>> GetParticipant(int eventId, int participantId)
    {
        var participant = await _context.EventParticipants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == participantId && p.EventId == eventId);

        if (participant is null)
            return NotFound();

        return Ok(new EventParticipantResponse(participant.Id, participant.EventId, participant.TeamId, participant.PlayerId, participant.Side));
    }

    [HttpPost("{eventId:int}/participants")]
    public async Task<ActionResult<EventParticipantResponse>> AddParticipant(int eventId, CreateEventParticipantRequest request)
    {
        if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            return NotFound();

        // A participant references a team or a player (at least one must be set).
        if (request.TeamId is null && request.PlayerId is null)
            ModelState.AddModelError(nameof(request.TeamId), "A participant must reference a team or a player.");

        if (request.TeamId is int teamId && !await _context.Teams.AnyAsync(t => t.Id == teamId))
            ModelState.AddModelError(nameof(request.TeamId), "Team does not exist.");

        if (request.PlayerId is int playerId && !await _context.Players.AnyAsync(p => p.Id == playerId))
            ModelState.AddModelError(nameof(request.PlayerId), "Player does not exist.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var participant = new EventParticipant
        {
            EventId = eventId,
            TeamId = request.TeamId,
            PlayerId = request.PlayerId,
            Side = request.Side
        };

        _context.EventParticipants.Add(participant);
        await _context.SaveChangesAsync();

        var response = new EventParticipantResponse(participant.Id, participant.EventId, participant.TeamId, participant.PlayerId, participant.Side);
        return CreatedAtAction(nameof(GetParticipant), new { eventId, participantId = participant.Id }, response);
    }

    [HttpDelete("{eventId:int}/participants/{participantId:int}")]
    public async Task<IActionResult> RemoveParticipant(int eventId, int participantId)
    {
        var participant = await _context.EventParticipants
            .FirstOrDefaultAsync(p => p.Id == participantId && p.EventId == eventId);

        if (participant is null)
            return NotFound();

        _context.EventParticipants.Remove(participant);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
