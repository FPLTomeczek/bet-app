using System.ComponentModel.DataAnnotations;

namespace BetApp.Api.Dtos;

public record SportCategoryResponse(int Id, string Name);

// Validation attributes sit on the constructor parameter (no `property:` target) — MVC
// reads a record's validation metadata from the primary constructor.
public record CreateSportCategoryRequest(
    [Required]
    [MaxLength(50)]
    string Name);

public record UpdateSportCategoryRequest(
    [Required]
    [MaxLength(50)]
    string Name);
