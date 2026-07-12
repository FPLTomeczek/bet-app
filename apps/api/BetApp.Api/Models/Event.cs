namespace BetApp.Api.Models;

public class Event
{
    public int Id { get; set; }
    public int SportCategoryId { get; set; }
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Scheduled;
    public string? Result { get; set; }

    public SportCategory? SportCategory { get; set; }
    public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
}
