using Nebula.Api.Helpers;
using Nebula.Application.Common;
using Nebula.Application.Services;

namespace Nebula.Api.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/my/tasks", async (
            int? limit, TaskService svc, ICurrentUserService user, CancellationToken ct) =>
            Results.Ok(await svc.GetMyTasksAsync(user.Subject, limit ?? 10, ct)))
            .WithTags("Tasks").RequireAuthorization();

        app.MapGet("/api/tasks/{taskId:guid}", async (
            Guid taskId, TaskService svc, CancellationToken ct) =>
        {
            var result = await svc.GetByIdAsync(taskId, ct);
            return result is null ? ProblemDetailsHelper.NotFound("Task", taskId) : Results.Ok(result);
        }).WithTags("Tasks").RequireAuthorization();

        // F0003 write endpoints NOT registered — return 404 by default

        return app;
    }
}
