using Nebula.Application.Common;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;

namespace Nebula.Application.Services;

public class RenewalService(
    IRenewalRepository renewalRepo,
    IWorkflowTransitionRepository transitionRepo,
    ITimelineRepository timelineRepo)
{
    public async Task<RenewalDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var renewal = await renewalRepo.GetByIdAsync(id, ct);
        return renewal is null ? null : MapToDto(renewal);
    }

    public async Task<IReadOnlyList<WorkflowTransitionRecordDto>> GetTransitionsAsync(
        Guid renewalId, CancellationToken ct = default)
    {
        var transitions = await transitionRepo.ListByEntityAsync("Renewal", renewalId, ct);
        return transitions.Select(MapTransition).ToList();
    }

    public async Task<(WorkflowTransitionRecordDto? Dto, string? ErrorCode)> TransitionAsync(
        Guid renewalId, WorkflowTransitionRequestDto dto, ICurrentUserService user, CancellationToken ct = default)
    {
        var renewal = await renewalRepo.GetByIdAsync(renewalId, ct);
        if (renewal is null) return (null, "not_found");

        if (!WorkflowStateMachine.IsValidTransition("Renewal", renewal.CurrentStatus, dto.ToState))
            return (null, "invalid_transition");

        var now = DateTime.UtcNow;
        var transition = new WorkflowTransition
        {
            WorkflowType = "Renewal",
            EntityId = renewalId,
            FromState = renewal.CurrentStatus,
            ToState = dto.ToState,
            Reason = dto.Reason,
            ActorSubject = user.Subject,
            OccurredAt = now,
        };

        renewal.CurrentStatus = dto.ToState;
        renewal.UpdatedAt = now;
        renewal.UpdatedBy = user.Subject;

        await transitionRepo.AddAsync(transition, ct);
        await renewalRepo.UpdateAsync(renewal, ct);

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Renewal",
            EntityId = renewalId,
            EventType = "RenewalTransitioned",
            EventDescription = $"Renewal transitioned from {transition.FromState} to {transition.ToState}",
            ActorSubject = user.Subject,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        return (MapTransition(transition), null);
    }

    private static RenewalDto MapToDto(Renewal r) => new(
        r.Id, r.AccountId, r.BrokerId, r.SubmissionId, r.CurrentStatus,
        r.RenewalDate, r.AssignedTo, r.CreatedAt, r.CreatedBy, r.UpdatedAt, r.UpdatedBy);

    private static WorkflowTransitionRecordDto MapTransition(WorkflowTransition t) => new(
        t.Id, t.WorkflowType, t.EntityId, t.FromState, t.ToState, t.Reason, t.OccurredAt);
}
