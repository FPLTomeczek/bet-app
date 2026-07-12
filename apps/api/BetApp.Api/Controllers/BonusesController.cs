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
    public async Task<ActionResult<IEnumerable<BonusResponse>>> GetAll()
    {
        var bonuses = await _context.Bonuses
            .AsNoTracking()
            .Select(b => new BonusResponse(b.Id, b.Name, b.Type, b.Value, b.ValidFrom, b.ValidTo))
            .ToListAsync();

        return Ok(bonuses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BonusResponse>> GetById(int id)
    {
        var bonus = await _context.Bonuses.FindAsync(id);

        if (bonus is null)
            return NotFound();

        return Ok(new BonusResponse(bonus.Id, bonus.Name, bonus.Type, bonus.Value, bonus.ValidFrom, bonus.ValidTo));
    }

    [HttpPost]
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
