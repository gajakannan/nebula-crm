using FluentAssertions;
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

        result.Should().NotBeNull();
        result!.Id.Should().Be(submission.Id);
        result.CurrentStatus.Should().Be("Received");
        result.LineOfBusiness.Should().Be("Cyber");
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

        result.Should().ContainSingle();
        result[0].WorkflowType.Should().Be("Submission");
        result[0].Reason.Should().Be("Initial review");
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

        result.Should().BeNull();
        error.Should().Be("not_found");
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

        result.Should().BeNull();
        error.Should().Be("invalid_transition");
        seeded.CurrentStatus.Should().Be("Received");
        _transitionRepo.Items.Should().BeEmpty();
        _timelineRepo.Events.Should().BeEmpty();
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

        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.FromState.Should().Be("Received");
        result.ToState.Should().Be("Triaging");
        seeded.CurrentStatus.Should().Be("Triaging");
        seeded.UpdatedByUserId.Should().Be(_user.UserId);
        _transitionRepo.Items.Should().ContainSingle();
        _timelineRepo.Events.Should().ContainSingle(e =>
            e.EntityType == "Submission" &&
            e.EventType == "SubmissionTransitioned" &&
            e.ActorUserId == _user.UserId);
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

        result.Should().NotBeNull();
        result!.Id.Should().Be(renewal.Id);
        result.CurrentStatus.Should().Be("Created");
        result.LineOfBusiness.Should().Be("Property");
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

        result.Should().ContainSingle();
        result[0].WorkflowType.Should().Be("Renewal");
        result[0].ToState.Should().Be("DataReview");
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

        result.Should().BeNull();
        error.Should().Be("not_found");
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

        result.Should().BeNull();
        error.Should().Be("invalid_transition");
        seeded.CurrentStatus.Should().Be("Created");
        _transitionRepo.Items.Should().BeEmpty();
        _timelineRepo.Events.Should().BeEmpty();
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

        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.FromState.Should().Be("Created");
        result.ToState.Should().Be("DataReview");
        seeded.CurrentStatus.Should().Be("DataReview");
        _transitionRepo.Items.Should().ContainSingle();
        _timelineRepo.Events.Should().ContainSingle(e =>
            e.EntityType == "Renewal" &&
            e.EventType == "RenewalTransitioned");
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
