using Nebula.Domain.Entities;

namespace Nebula.Application.Interfaces;

public interface ITimelineRepository
{
    Task<IReadOnlyList<ActivityTimelineEvent>> ListEventsAsync(string entityType, Guid? entityId, int limit, CancellationToken ct = default);
    Task AddEventAsync(ActivityTimelineEvent evt, CancellationToken ct = default);
}
