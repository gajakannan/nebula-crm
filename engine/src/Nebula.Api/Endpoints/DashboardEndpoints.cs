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

        group.MapGet("/opportunities", async (DashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetOpportunitiesAsync(ct)));

        group.MapGet("/opportunities/flow", async (
            string entityType,
            int? periodDays,
            DashboardService svc,
            CancellationToken ct) =>
        {
            if (entityType is not ("submission" or "renewal"))
            {
                return Results.BadRequest(new
                {
                    code = "invalid_entity_type",
                    message = "entityType must be 'submission' or 'renewal'.",
                });
            }

            return Results.Ok(await svc.GetOpportunityFlowAsync(entityType, periodDays ?? 180, ct));
        });

        group.MapGet("/opportunities/{entityType}/{status}/items", async (
            string entityType, string status, DashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetOpportunityItemsAsync(entityType, status, ct)));

        group.MapGet("/nudges", async (DashboardService svc, ICurrentUserService user, CancellationToken ct) =>
            Results.Ok(await svc.GetNudgesAsync(user.Subject, ct)));

        return app;
    }
}
