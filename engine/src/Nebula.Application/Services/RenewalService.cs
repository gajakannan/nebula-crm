using Nebula.Application.Common;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;

namespace Nebula.Application.Services;

public class RenewalService(
    IRenewalRepository renewalRepo,
    IWorkflowTransitionRepository transitionRepo,
    ITimelineRepository timelineRepo,
    IUnitOfWork unitOfWork)
{
    public async Task<RenewalDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var renewal = await renewalRepo.GetByIdWithIncludesAsync(id, ct);
        return renewal is null ? null : MapToDto(renewal);
    }

    public async Task<IReadOnlyList<WorkflowTransitionRecordDto>> GetTransitionsAsync(
        Guid renewalId, CancellationToken ct = default)
    {
        var transitions = await transitionRepo.ListByEntityAsync("Renewal", renewalId, ct);
        return transitions.Select(MapTransition).ToList();
    }

    public async Task<(WorkflowTransitionRecordDto? Dto, string? ErrorCode, IReadOnlyList<string>? MissingItems)> TransitionAsync(
        Guid renewalId, WorkflowTransitionRequestDto dto, ICurrentUserService user, CancellationToken ct = default)
    {
        var renewal = await renewalRepo.GetByIdAsync(renewalId, ct);
        if (renewal is null) return (null, "not_found", null);

        if (!WorkflowStateMachine.IsValidTransition("Renewal", renewal.CurrentStatus, dto.ToState))
            return (null, "invalid_transition", null);

        if (string.Equals(dto.ToState, "Lost", StringComparison.Ordinal))
        {
            var missingItems = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.ReasonCode))
                missingItems.Add("reasonCode is required when transitioning to Lost");
            if (string.Equals(dto.ReasonCode, "Other", StringComparison.Ordinal)
                && string.IsNullOrWhiteSpace(dto.ReasonDetail))
            {
                missingItems.Add("reasonDetail is required when reasonCode is Other");
            }

            if (missingItems.Count > 0)
                return (null, "missing_transition_prerequisite", missingItems);

            renewal.LostReasonCode = dto.ReasonCode;
            renewal.LostReasonDetail = dto.ReasonDetail;
            renewal.BoundPolicyId = null;
            renewal.RenewalSubmissionId = null;
        }

        if (string.Equals(dto.ToState, "Completed", StringComparison.Ordinal))
        {
            if (!dto.BoundPolicyId.HasValue && !dto.RenewalSubmissionId.HasValue)
            {
                return (null, "missing_transition_prerequisite", ["boundPolicyId or renewalSubmissionId is required when transitioning to Completed"]);
            }

            renewal.BoundPolicyId = dto.BoundPolicyId;
            renewal.RenewalSubmissionId = dto.RenewalSubmissionId;
        }

        var now = DateTime.UtcNow;
        var transition = new WorkflowTransition
        {
            WorkflowType = "Renewal",
            EntityId = renewalId,
            FromState = renewal.CurrentStatus,
            ToState = dto.ToState,
            Reason = dto.Reason ?? dto.ReasonCode,
            ActorUserId = user.UserId,
            OccurredAt = now,
        };

        renewal.CurrentStatus = dto.ToState;
        renewal.UpdatedAt = now;
        renewal.UpdatedByUserId = user.UserId;

        await transitionRepo.AddAsync(transition, ct);
        await renewalRepo.UpdateAsync(renewal, ct);

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Renewal",
            EntityId = renewalId,
            EventType = "RenewalTransitioned",
            EventDescription = $"Renewal transitioned from {transition.FromState} to {transition.ToState}",
            ActorUserId = user.UserId,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        await unitOfWork.CommitAsync(ct);

        return (MapTransition(transition), null, null);
    }

    private static RenewalDto MapToDto(Renewal r) => new(
        r.Id,
        r.AccountId,
        r.BrokerId,
        r.PolicyId,
        r.LineOfBusiness,
        r.CurrentStatus,
        r.PolicyExpirationDate,
        r.TargetOutreachDate,
        r.AssignedToUserId,
        r.LostReasonCode,
        r.LostReasonDetail,
        r.BoundPolicyId,
        r.RenewalSubmissionId,
        GetUrgency(r),
        WorkflowStateMachine.GetAvailableTransitions("Renewal", r.CurrentStatus),
        r.AssignedToUser?.DisplayName,
        r.Account?.Name,
        r.Broker?.LegalName,
        r.RowVersion.ToString(),
        r.CreatedAt,
        r.CreatedByUserId,
        r.UpdatedAt,
        r.UpdatedByUserId);

    private static WorkflowTransitionRecordDto MapTransition(WorkflowTransition t) => new(
        t.Id, t.WorkflowType, t.EntityId, t.FromState, t.ToState, t.Reason, t.OccurredAt);

    private static string? GetUrgency(Renewal renewal)
    {
        if (!string.Equals(renewal.CurrentStatus, "Identified", StringComparison.Ordinal))
            return null;

        return DateTime.UtcNow.Date > renewal.TargetOutreachDate.Date
            ? "overdue"
            : null;
    }
}
