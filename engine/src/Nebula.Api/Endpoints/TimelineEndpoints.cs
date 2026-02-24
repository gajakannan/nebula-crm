using Nebula.Application.Services;

namespace Nebula.Api.Endpoints;

public static class TimelineEndpoints
{
    public static IEndpointRouteBuilder MapTimelineEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/timeline/events", async (
            string entityType, Guid? entityId, int? limit,
            TimelineService svc, CancellationToken ct) =>
            Results.Ok(await svc.ListEventsAsync(entityType, entityId, limit ?? 20, ct)))
            .WithTags("Timeline").RequireAuthorization();

        return app;
    }
}
