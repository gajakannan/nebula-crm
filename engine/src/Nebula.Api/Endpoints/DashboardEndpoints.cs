using Nebula.Application.Common;
using Nebula.Application.Services;

namespace Nebula.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/kpis", async (DashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetKpisAsync(ct)));

        group.MapGet("/pipeline", async (DashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetPipelineAsync(ct)));

        group.MapGet("/pipeline/{entityType}/{status}/items", async (
            string entityType, string status, DashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetPipelineItemsAsync(entityType, status, ct)));

        group.MapGet("/nudges", async (DashboardService svc, ICurrentUserService user, CancellationToken ct) =>
            Results.Ok(await svc.GetNudgesAsync(user.Subject, ct)));

        return app;
    }
}
