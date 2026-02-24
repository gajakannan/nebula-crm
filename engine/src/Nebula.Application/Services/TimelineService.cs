using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;

namespace Nebula.Application.Services;

public class TimelineService(ITimelineRepository timelineRepo)
{
    public async Task<IReadOnlyList<TimelineEventDto>> ListEventsAsync(
        string entityType, Guid? entityId, int limit, CancellationToken ct = default)
    {
        var events = await timelineRepo.ListEventsAsync(entityType, entityId, limit, ct);
        return events.Select(e => new TimelineEventDto(
            e.Id, e.EntityType, e.EntityId, e.EventType,
            e.EventDescription, null, e.ActorDisplayName, e.OccurredAt))
            .ToList();
    }
}
