namespace Nebula.Domain.Entities;

public class ActivityTimelineEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string EventType { get; set; } = default!;
    public string? EventPayloadJson { get; set; }
    public string EventDescription { get; set; } = default!;
    public string ActorSubject { get; set; } = default!;
    public string? ActorDisplayName { get; set; }
    public DateTime OccurredAt { get; set; }
}
