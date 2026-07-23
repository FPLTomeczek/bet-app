using System.Text.Json.Serialization;

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

// String-serialized (name, not int) in JSON and in the OpenAPI schema via the
// [JsonConverter] on the type — so the wire format and the generated contract
// can't drift. Persisted as string via HasConversion<string>() in the config.
[JsonConverter(typeof(JsonStringEnumConverter<EventStatus>))]
public enum EventStatus { Scheduled, Live, Finished, Cancelled }
