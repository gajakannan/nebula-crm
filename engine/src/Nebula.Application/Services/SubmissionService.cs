using Nebula.Application.Common;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;

namespace Nebula.Application.Services;

public class SubmissionService(
    ISubmissionRepository submissionRepo,
    IWorkflowTransitionRepository transitionRepo,
    ITimelineRepository timelineRepo)
{
    public async Task<SubmissionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var sub = await submissionRepo.GetByIdAsync(id, ct);
        return sub is null ? null : MapToDto(sub);
    }

    public async Task<IReadOnlyList<WorkflowTransitionRecordDto>> GetTransitionsAsync(
        Guid submissionId, CancellationToken ct = default)
    {
        var transitions = await transitionRepo.ListByEntityAsync("Submission", submissionId, ct);
        return transitions.Select(MapTransition).ToList();
    }

    public async Task<(WorkflowTransitionRecordDto? Dto, string? ErrorCode)> TransitionAsync(
        Guid submissionId, WorkflowTransitionRequestDto dto, ICurrentUserService user, CancellationToken ct = default)
    {
        var submission = await submissionRepo.GetByIdAsync(submissionId, ct);
        if (submission is null) return (null, "not_found");

        if (!WorkflowStateMachine.IsValidTransition("Submission", submission.CurrentStatus, dto.ToState))
            return (null, "invalid_transition");

        var now = DateTime.UtcNow;
        var transition = new WorkflowTransition
        {
            WorkflowType = "Submission",
            EntityId = submissionId,
            FromState = submission.CurrentStatus,
            ToState = dto.ToState,
            Reason = dto.Reason,
            ActorSubject = user.Subject,
            OccurredAt = now,
        };

        submission.CurrentStatus = dto.ToState;
        submission.UpdatedAt = now;
        submission.UpdatedBy = user.Subject;

        await transitionRepo.AddAsync(transition, ct);
        await submissionRepo.UpdateAsync(submission, ct);

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Submission",
            EntityId = submissionId,
            EventType = "SubmissionTransitioned",
            EventDescription = $"Submission transitioned from {transition.FromState} to {transition.ToState}",
            ActorSubject = user.Subject,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        return (MapTransition(transition), null);
    }

    private static SubmissionDto MapToDto(Submission s) => new(
        s.Id, s.AccountId, s.BrokerId, s.ProgramId, s.CurrentStatus,
        s.EffectiveDate, s.PremiumEstimate, s.AssignedTo,
        s.CreatedAt, s.CreatedBy, s.UpdatedAt, s.UpdatedBy);

    private static WorkflowTransitionRecordDto MapTransition(WorkflowTransition t) => new(
        t.Id, t.WorkflowType, t.EntityId, t.FromState, t.ToState, t.Reason, t.OccurredAt);
}
