using BetApp.Api.Data;
using BetApp.Api.Dtos;
using BetApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SportCategoriesController : ControllerBase
{
    private readonly BetAppContext _context;

    public SportCategoriesController(BetAppContext context)
    {
        _context = context;
    }

    // GET api/sportcategories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SportCategoryResponse>>> GetAll()
    {
        // The projection to a DTO runs on the database side: SELECT "Id","Name".
        var categories = await _context.SportCategories
            .AsNoTracking()
            .Select(c => new SportCategoryResponse(c.Id, c.Name))
            .ToListAsync();

        return Ok(categories);
    }

    // GET api/sportcategories/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SportCategoryResponse>> GetById(int id)
    {
        var category = await _context.SportCategories.FindAsync(id);

        if (category is null)
            return NotFound();

        return Ok(new SportCategoryResponse(category.Id, category.Name));
    }

    // POST api/sportcategories
    [HttpPost]
    public async Task<ActionResult<SportCategoryResponse>> Create(CreateSportCategoryRequest request)
    {
        // Name has a unique index — pre-check to return a clean 400 instead of
        // letting the DB constraint surface as a raw 500.
        if (await _context.SportCategories.AnyAsync(c => c.Name == request.Name))
        {
            ModelState.AddModelError(nameof(request.Name), "A category with this name already exists.");
            return ValidationProblem(ModelState);
        }

        var category = new SportCategory { Name = request.Name };

        _context.SportCategories.Add(category);
        await _context.SaveChangesAsync();

        var response = new SportCategoryResponse(category.Id, category.Name);

        // 201 Created + Location header pointing to GetById of the new resource.
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
    }

    // PUT api/sportcategories/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSportCategoryRequest request)
    {
        var category = await _context.SportCategories.FindAsync(id);

        if (category is null)
            return NotFound();

        if (await _context.SportCategories.AnyAsync(c => c.Name == request.Name && c.Id != id))
        {
            ModelState.AddModelError(nameof(request.Name), "A category with this name already exists.");
            return ValidationProblem(ModelState);
        }

        // EF tracks the entity loaded via FindAsync — changing a field is enough,
        // SaveChanges will emit an UPDATE only for the modified columns.
        category.Name = request.Name;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/sportcategories/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.SportCategories.FindAsync(id);

        if (category is null)
            return NotFound();

        _context.SportCategories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
