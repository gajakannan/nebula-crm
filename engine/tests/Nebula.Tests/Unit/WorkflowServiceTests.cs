using Shouldly;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Application.Services;
using Nebula.Domain.Entities;

namespace Nebula.Tests.Unit;

public class WorkflowServiceTests
{
    private readonly StubWorkflowTransitionRepository _transitionRepo = new();
    private readonly StubTimelineRepository _timelineRepo = new();
    private readonly StubCurrentUserService _user = new(Guid.Parse("bbbb0000-0000-0000-0000-000000000001"));

    [Fact]
    public async Task SubmissionService_GetByIdAsync_MapsDto()
    {
        var repo = new StubSubmissionRepository();
        var now = DateTime.UtcNow;
        var submission = new Submission
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            ProgramId = Guid.NewGuid(),
            LineOfBusiness = "Cyber",
            CurrentStatus = "Received",
            EffectiveDate = now.Date.AddDays(30),
            PremiumEstimate = 125000m,
            AssignedToUserId = _user.UserId,
            CreatedAt = now,
            CreatedByUserId = _user.UserId,
            UpdatedAt = now,
            UpdatedByUserId = _user.UserId,
        };
        repo.Seed(submission);

        var service = new SubmissionService(repo, _transitionRepo, _timelineRepo);

        var result = await service.GetByIdAsync(submission.Id);

        result.ShouldNotBeNull();
        result!.Id.ShouldBe(submission.Id);
        result.CurrentStatus.ShouldBe("Received");
        result.LineOfBusiness.ShouldBe("Cyber");
    }

    [Fact]
    public async Task SubmissionService_GetTransitionsAsync_MapsTransitions()
    {
        var repo = new StubSubmissionRepository();
        var submissionId = Guid.NewGuid();
        var occurredAt = DateTime.UtcNow.AddDays(-1);
        _transitionRepo.Seed(new WorkflowTransition
        {
            WorkflowType = "Submission",
            EntityId = submissionId,
            FromState = "Received",
            ToState = "Triaging",
            Reason = "Initial review",
            ActorUserId = _user.UserId,
            OccurredAt = occurredAt,
        });

        var service = new SubmissionService(repo, _transitionRepo, _timelineRepo);

        var result = await service.GetTransitionsAsync(submissionId);

        result.Count().ShouldBe(1);
        result[0].WorkflowType.ShouldBe("Submission");
        result[0].Reason.ShouldBe("Initial review");
    }

    [Fact]
    public async Task SubmissionService_TransitionAsync_NotFound_ReturnsNotFound()
    {
        var repo = new StubSubmissionRepository();
        var service = new SubmissionService(repo, _transitionRepo, _timelineRepo);

        var (result, error) = await service.TransitionAsync(
            Guid.NewGuid(),
            new WorkflowTransitionRequestDto("Triaging", "review"),
            _user);

        result.ShouldBeNull();
        error.ShouldBe("not_found");
    }

    [Fact]
    public async Task SubmissionService_TransitionAsync_InvalidTransition_ReturnsInvalidTransition()
    {
        var repo = new StubSubmissionRepository();
        repo.Seed(new Submission
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            CurrentStatus = "Received",
            EffectiveDate = DateTime.UtcNow.Date,
            PremiumEstimate = 50000m,
            AssignedToUserId = _user.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _user.UserId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedByUserId = _user.UserId,
        });

        var service = new SubmissionService(repo, _transitionRepo, _timelineRepo);
        var seeded = repo.Single();

        var (result, error) = await service.TransitionAsync(
            seeded.Id,
            new WorkflowTransitionRequestDto("Binding", "too early"),
            _user);

        result.ShouldBeNull();
        error.ShouldBe("invalid_transition");
        seeded.CurrentStatus.ShouldBe("Received");
        _transitionRepo.Items.ShouldBeEmpty();
        _timelineRepo.Events.ShouldBeEmpty();
    }

    [Fact]
    public async Task SubmissionService_TransitionAsync_ValidTransition_PersistsTransitionAndTimeline()
    {
        var repo = new StubSubmissionRepository();
        repo.Seed(new Submission
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            CurrentStatus = "Received",
            EffectiveDate = DateTime.UtcNow.Date,
            PremiumEstimate = 50000m,
            AssignedToUserId = _user.UserId,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = _user.UserId,
            UpdatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedByUserId = _user.UserId,
        });

        var service = new SubmissionService(repo, _transitionRepo, _timelineRepo);
        var seeded = repo.Single();

        var (result, error) = await service.TransitionAsync(
            seeded.Id,
            new WorkflowTransitionRequestDto("Triaging", "queue"),
            _user);

        error.ShouldBeNull();
        result.ShouldNotBeNull();
        result!.FromState.ShouldBe("Received");
        result.ToState.ShouldBe("Triaging");
        seeded.CurrentStatus.ShouldBe("Triaging");
        seeded.UpdatedByUserId.ShouldBe(_user.UserId);
        _transitionRepo.Items.Count.ShouldBe(1);
        var timelineEvent = _timelineRepo.Events.ShouldHaveSingleItem();
        timelineEvent.EntityType.ShouldBe("Submission");
        timelineEvent.EventType.ShouldBe("SubmissionTransitioned");
        timelineEvent.ActorUserId.ShouldBe(_user.UserId);
    }

    [Fact]
    public async Task RenewalService_GetByIdAsync_MapsDto()
    {
        var repo = new StubRenewalRepository();
        var now = DateTime.UtcNow;
        var renewal = new Renewal
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            LineOfBusiness = "Property",
            CurrentStatus = "Created",
            RenewalDate = now.Date.AddDays(45),
            AssignedToUserId = _user.UserId,
            CreatedAt = now,
            CreatedByUserId = _user.UserId,
            UpdatedAt = now,
            UpdatedByUserId = _user.UserId,
        };
        repo.Seed(renewal);

        var service = new RenewalService(repo, _transitionRepo, _timelineRepo);

        var result = await service.GetByIdAsync(renewal.Id);

        result.ShouldNotBeNull();
        result!.Id.ShouldBe(renewal.Id);
        result.CurrentStatus.ShouldBe("Created");
        result.LineOfBusiness.ShouldBe("Property");
    }

    [Fact]
    public async Task RenewalService_GetTransitionsAsync_MapsTransitions()
    {
        var repo = new StubRenewalRepository();
        var renewalId = Guid.NewGuid();
        _transitionRepo.Seed(new WorkflowTransition
        {
            WorkflowType = "Renewal",
            EntityId = renewalId,
            FromState = "Created",
            ToState = "DataReview",
            Reason = "ready",
            ActorUserId = _user.UserId,
            OccurredAt = DateTime.UtcNow,
        });

        var service = new RenewalService(repo, _transitionRepo, _timelineRepo);

        var result = await service.GetTransitionsAsync(renewalId);

        result.Count().ShouldBe(1);
        result[0].WorkflowType.ShouldBe("Renewal");
        result[0].ToState.ShouldBe("DataReview");
    }

    [Fact]
    public async Task RenewalService_TransitionAsync_NotFound_ReturnsNotFound()
    {
        var repo = new StubRenewalRepository();
        var service = new RenewalService(repo, _transitionRepo, _timelineRepo);

        var (result, error) = await service.TransitionAsync(
            Guid.NewGuid(),
            new WorkflowTransitionRequestDto("DataReview", "seed"),
            _user);

        result.ShouldBeNull();
        error.ShouldBe("not_found");
    }

    [Fact]
    public async Task RenewalService_TransitionAsync_InvalidTransition_ReturnsInvalidTransition()
    {
        var repo = new StubRenewalRepository();
        repo.Seed(new Renewal
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            CurrentStatus = "Created",
            RenewalDate = DateTime.UtcNow.Date.AddDays(30),
            AssignedToUserId = _user.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _user.UserId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedByUserId = _user.UserId,
        });

        var service = new RenewalService(repo, _transitionRepo, _timelineRepo);
        var seeded = repo.Single();

        var (result, error) = await service.TransitionAsync(
            seeded.Id,
            new WorkflowTransitionRequestDto("Negotiation", "too early"),
            _user);

        result.ShouldBeNull();
        error.ShouldBe("invalid_transition");
        seeded.CurrentStatus.ShouldBe("Created");
        _transitionRepo.Items.ShouldBeEmpty();
        _timelineRepo.Events.ShouldBeEmpty();
    }

    [Fact]
    public async Task RenewalService_TransitionAsync_ValidTransition_PersistsTransitionAndTimeline()
    {
        var repo = new StubRenewalRepository();
        repo.Seed(new Renewal
        {
            AccountId = Guid.NewGuid(),
            BrokerId = Guid.NewGuid(),
            CurrentStatus = "Created",
            RenewalDate = DateTime.UtcNow.Date.AddDays(30),
            AssignedToUserId = _user.UserId,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedByUserId = _user.UserId,
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedByUserId = _user.UserId,
        });

        var service = new RenewalService(repo, _transitionRepo, _timelineRepo);
        var seeded = repo.Single();

        var (result, error) = await service.TransitionAsync(
            seeded.Id,
            new WorkflowTransitionRequestDto("DataReview", "start"),
            _user);

        error.ShouldBeNull();
        result.ShouldNotBeNull();
        result!.FromState.ShouldBe("Created");
        result.ToState.ShouldBe("DataReview");
        seeded.CurrentStatus.ShouldBe("DataReview");
        _transitionRepo.Items.Count.ShouldBe(1);
        var timelineEvent = _timelineRepo.Events.ShouldHaveSingleItem();
        timelineEvent.EntityType.ShouldBe("Renewal");
        timelineEvent.EventType.ShouldBe("RenewalTransitioned");
    }
}

internal sealed class StubSubmissionRepository : ISubmissionRepository
{
    private readonly Dictionary<Guid, Submission> _submissions = new();

    public void Seed(Submission submission) => _submissions[submission.Id] = submission;
    public Submission Single() => _submissions.Values.Single();

    public Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_submissions.GetValueOrDefault(id));

    public Task UpdateAsync(Submission submission, CancellationToken ct = default)
    {
        _submissions[submission.Id] = submission;
        return Task.CompletedTask;
    }
}

internal sealed class StubRenewalRepository : IRenewalRepository
{
    private readonly Dictionary<Guid, Renewal> _renewals = new();

    public void Seed(Renewal renewal) => _renewals[renewal.Id] = renewal;
    public Renewal Single() => _renewals.Values.Single();

    public Task<Renewal?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_renewals.GetValueOrDefault(id));

    public Task UpdateAsync(Renewal renewal, CancellationToken ct = default)
    {
        _renewals[renewal.Id] = renewal;
        return Task.CompletedTask;
    }
}

internal sealed class StubWorkflowTransitionRepository : IWorkflowTransitionRepository
{
    public List<WorkflowTransition> Items { get; } = [];

    public void Seed(WorkflowTransition transition) => Items.Add(transition);

    public Task<IReadOnlyList<WorkflowTransition>> ListByEntityAsync(string workflowType, Guid entityId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<WorkflowTransition>>(Items
            .Where(t => t.WorkflowType == workflowType && t.EntityId == entityId)
            .ToList());

    public Task AddAsync(WorkflowTransition transition, CancellationToken ct = default)
    {
        Items.Add(transition);
        return Task.CompletedTask;
    }
}
