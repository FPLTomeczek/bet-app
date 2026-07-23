using System.Text.Json.Serialization;

namespace BetApp.Api.Models;

public class EventParticipant
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int? TeamId { get; set; }
    public int? PlayerId { get; set; }
    public ParticipantSide? Side { get; set; }

    public Event? Event { get; set; }
    public Team? Team { get; set; }
    public Player? Player { get; set; }
}

// String-serialized in JSON and OpenAPI via [JsonConverter] on the type.
[JsonConverter(typeof(JsonStringEnumConverter<ParticipantSide>))]
public enum ParticipantSide { Home, Away }
