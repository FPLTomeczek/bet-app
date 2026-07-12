using BetApp.Api.Dtos;
using BetApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetApp.Api.Controllers;

// A coupon is the aggregate root: it is placed together with its selections in a
// single request, and its financial figures (total odds, potential payout) are
// computed by the server. Selections are not edited independently, so there is no
// standalone coupon_selection controller. PUT/DELETE are intentionally omitted —
// a placed coupon changes only through settlement, which would be a dedicated
// state-transition endpoint rather than a free-form edit.
//
// This controller is a thin translator: it delegates all domain work to
// CouponService and only maps the outcome onto HTTP responses.
[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly CouponService _couponService;

    public CouponsController(CouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CouponResponse>>> GetAll()
    {
        return Ok(await _couponService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CouponResponse>> GetById(int id)
    {
        var coupon = await _couponService.GetByIdAsync(id);

        if (coupon is null)
            return NotFound();

        return Ok(coupon);
    }

    [HttpPost]
    public async Task<ActionResult<CouponResponse>> Create(CreateCouponRequest request)
    {
        var result = await _couponService.PlaceCouponAsync(request);

        if (!result.IsSuccess)
        {
            // Translate domain validation errors into the HTTP-shaped response.
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Field, error.Message);

            return ValidationProblem(ModelState);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}
