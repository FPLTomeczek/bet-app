using BetApp.Api.Data;
using BetApp.Api.Dtos;
using BetApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetApp.Api.Controllers;

// A coupon is the aggregate root: it is placed together with its selections in a
// single request, and its financial figures (total odds, potential payout) are
// computed by the server. Selections are not edited independently, so there is no
// standalone coupon_selection controller. PUT/DELETE are intentionally omitted —
// a placed coupon changes only through settlement, which would be a dedicated
// state-transition endpoint rather than a free-form edit.
[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly BetAppContext _context;

    public CouponsController(BetAppContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CouponResponse>>> GetAll()
    {
        var coupons = await _context.Coupons
            .AsNoTracking()
            .Select(MapToResponse)
            .ToListAsync();

        return Ok(coupons);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CouponResponse>> GetById(int id)
    {
        var coupon = await _context.Coupons
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(MapToResponse)
            .FirstOrDefaultAsync();

        if (coupon is null)
            return NotFound();

        return Ok(coupon);
    }

    [HttpPost]
    public async Task<ActionResult<CouponResponse>> Create(CreateCouponRequest request)
    {
        if (!await _context.AppUsers.AnyAsync(u => u.Id == request.UserId))
            ModelState.AddModelError(nameof(request.UserId), "User does not exist.");

        if (request.BonusId is int bonusId && !await _context.Bonuses.AnyAsync(b => b.Id == bonusId))
            ModelState.AddModelError(nameof(request.BonusId), "Bonus does not exist.");

        var eventIds = request.Selections.Select(s => s.EventId).Distinct().ToList();
        var existingEvents = await _context.Events.CountAsync(e => eventIds.Contains(e.Id));
        if (existingEvents != eventIds.Count)
            ModelState.AddModelError(nameof(request.Selections), "One or more selections reference a non-existent event.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Accumulator odds: the product of all selection odds. Payout = stake * odds.
        var totalOdds = Math.Round(
            request.Selections.Aggregate(1m, (acc, s) => acc * s.Odds),
            3, MidpointRounding.AwayFromZero);
        var potentialPayout = Math.Round(request.Stake * totalOdds, 2, MidpointRounding.AwayFromZero);

        var coupon = new Coupon
        {
            UserId = request.UserId,
            BonusId = request.BonusId,
            Stake = request.Stake,
            TotalOdds = totalOdds,
            PotentialPayout = potentialPayout,
            // Status defaults to Placed; PlacedAt is set by the database (now()).
            Selections = request.Selections
                .Select(s => new CouponSelection
                {
                    EventId = s.EventId,
                    Market = s.Market,
                    Selection = s.Selection,
                    Odds = s.Odds
                    // Result defaults to Open.
                })
                .ToList()
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        var response = await _context.Coupons
            .AsNoTracking()
            .Where(c => c.Id == coupon.Id)
            .Select(MapToResponse)
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, response);
    }

    // Reused projection so list, detail and create responses stay identical.
    private static readonly System.Linq.Expressions.Expression<Func<Coupon, CouponResponse>> MapToResponse =
        c => new CouponResponse(
            c.Id,
            c.UserId,
            c.BonusId,
            c.Stake,
            c.TotalOdds,
            c.PotentialPayout,
            c.Status,
            c.PlacedAt,
            c.Selections
                .Select(s => new CouponSelectionResponse(s.Id, s.EventId, s.Market, s.Selection, s.Odds, s.Result))
                .ToList());
}
