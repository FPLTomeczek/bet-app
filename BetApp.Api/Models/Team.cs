namespace BetApp.Api.Models;

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SportCategoryId { get; set; }
    public string? Country { get; set; }
    public string? ImageUrl { get; set; }

    public SportCategory? SportCategory { get; set; }
    public ICollection<Player> Players { get; set; } = new List<Player>();
}
