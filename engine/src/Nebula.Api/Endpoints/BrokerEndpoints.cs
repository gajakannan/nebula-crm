using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nebula.Api.Helpers;
using Nebula.Application.Common;
using Nebula.Application.DTOs;
using Nebula.Application.Services;

namespace Nebula.Api.Endpoints;

public static class BrokerEndpoints
{
    public static RouteGroupBuilder MapBrokerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/brokers")
            .WithTags("Brokers")
            .RequireAuthorization();

        group.MapGet("/", ListBrokers);
        group.MapPost("/", CreateBroker);
        group.MapGet("/{brokerId:guid}", GetBroker);
        group.MapPut("/{brokerId:guid}", UpdateBroker);
        group.MapDelete("/{brokerId:guid}", DeleteBroker);

        return group;
    }

    private static async Task<IResult> ListBrokers(
        string? q, string? status, int? page, int? pageSize,
        BrokerService svc, CancellationToken ct)
    {
        var result = await svc.ListAsync(q, status, page ?? 1, pageSize ?? 20, ct);
        return Results.Ok(new { data = result.Data, page = result.Page, pageSize = result.PageSize, totalCount = result.TotalCount, totalPages = result.TotalPages });
    }

    private static async Task<IResult> CreateBroker(
        BrokerCreateDto dto,
        IValidator<BrokerCreateDto> validator,
        BrokerService svc,
        ICurrentUserService user,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ProblemDetailsHelper.ValidationError(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

        var (result, error) = await svc.CreateAsync(dto, user, ct);
        return error switch
        {
            "duplicate_license" => ProblemDetailsHelper.DuplicateLicense(),
            _ => Results.Created($"/api/brokers/{result!.Id}", result),
        };
    }

    private static async Task<IResult> GetBroker(
        Guid brokerId, BrokerService svc, ICurrentUserService user, CancellationToken ct)
    {
        var result = await svc.GetByIdAsync(brokerId, user, ct);
        return result is null ? ProblemDetailsHelper.NotFound("Broker", brokerId) : Results.Ok(result);
    }

    private static async Task<IResult> UpdateBroker(
        Guid brokerId,
        BrokerUpdateDto dto,
        IValidator<BrokerUpdateDto> validator,
        BrokerService svc,
        ICurrentUserService user,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ProblemDetailsHelper.ValidationError(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

        var ifMatch = httpContext.Request.Headers.IfMatch.FirstOrDefault();
        if (string.IsNullOrEmpty(ifMatch) || !uint.TryParse(ifMatch.Trim('"'), out var rowVersion))
            return Results.Problem(title: "If-Match header required", statusCode: 428);

        try
        {
            var (result, error) = await svc.UpdateAsync(brokerId, dto, rowVersion, user, ct);
            return error switch
            {
                "not_found" => ProblemDetailsHelper.NotFound("Broker", brokerId),
                _ => Results.Ok(result),
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return ProblemDetailsHelper.ConcurrencyConflict();
        }
    }

    private static async Task<IResult> DeleteBroker(
        Guid brokerId, BrokerService svc, ICurrentUserService user, CancellationToken ct)
    {
        var error = await svc.DeleteAsync(brokerId, user, ct);
        return error switch
        {
            "not_found" => ProblemDetailsHelper.NotFound("Broker", brokerId),
            "active_submissions_exist" => ProblemDetailsHelper.ActiveSubmissionsExist(),
            _ => Results.NoContent(),
        };
    }
}
