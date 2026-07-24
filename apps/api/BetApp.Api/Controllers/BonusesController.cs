using BetApp.Api.Data;
using BetApp.Api.Dtos;
using BetApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BonusesController : ControllerBase
{
    private readonly BetAppContext _context;

    public BonusesController(BetAppContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<BonusResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BonusResponse>>> GetAll()
    {
        var bonuses = await _context.Bonuses
            .AsNoTracking()
            .Select(b => new BonusResponse(b.Id, b.Name, b.Type, b.Value, b.ValidFrom, b.ValidTo))
            .ToListAsync();

        return Ok(bonuses);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<BonusResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BonusResponse>> GetById(int id)
    {
        var bonus = await _context.Bonuses.FindAsync(id);

        if (bonus is null)
            return NotFound();

        return Ok(new BonusResponse(bonus.Id, bonus.Name, bonus.Type, bonus.Value, bonus.ValidFrom, bonus.ValidTo));
    }

    // No manual validation here, but the DTO carries DataAnnotations — [ApiController]
    // validates them before the action runs, so 400 is still a reachable outcome.
    [HttpPost]
    [ProducesResponseType<BonusResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BonusResponse>> Create(CreateBonusRequest request)
    {
        var bonus = new Bonus
        {
            Name = request.Name,
            Type = request.Type,
            Value = request.Value,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };

        _context.Bonuses.Add(bonus);
        await _context.SaveChangesAsync();

        var response = new BonusResponse(bonus.Id, bonus.Name, bonus.Type, bonus.Value, bonus.ValidFrom, bonus.ValidTo);
        return CreatedAtAction(nameof(GetById), new { id = bonus.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, UpdateBonusRequest request)
    {
        var bonus = await _context.Bonuses.FindAsync(id);

        if (bonus is null)
            return NotFound();

        bonus.Name = request.Name;
        bonus.Type = request.Type;
        bonus.Value = request.Value;
        bonus.ValidFrom = request.ValidFrom;
        bonus.ValidTo = request.ValidTo;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var bonus = await _context.Bonuses.FindAsync(id);

        if (bonus is null)
            return NotFound();

        _context.Bonuses.Remove(bonus);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
