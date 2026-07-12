using System.ComponentModel.DataAnnotations;

namespace BetApp.Api.Dtos;

// Response sent to the client. Deliberately decoupled from the entity
// so that changes to the database model don't accidentally change the API contract.
public record SportCategoryResponse(int Id, string Name);

// Create input. No Id field => the client cannot set it (protection against over-posting).
// Validation attributes must sit on the constructor parameter (no `property:` target),
// because MVC reads record validation metadata from the primary constructor.
public record CreateSportCategoryRequest(
    [Required]
    [MaxLength(50)]
    string Name);

// Separate type for update — identical to Create today, but keeping them apart
// lets them evolve independently (e.g. update could allow different fields than create).
public record UpdateSportCategoryRequest(
    [Required]
    [MaxLength(50)]
    string Name);
