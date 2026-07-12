namespace BetApp.Api.Models;

public class Player
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? TeamId { get; set; }
    public string? Nationality { get; set; }
    public string? Position { get; set; }
    public DateOnly? BirthDate { get; set; }

    public Team? Team { get; set; }
}
