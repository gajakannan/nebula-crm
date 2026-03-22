using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nebula.Application.DTOs;
using Nebula.Domain.Entities;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Tests.Integration;

[Collection(IntegrationTestCollection.Name)]
public class WorkflowEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetSubmission_Existing_Returns200()
    {
        var submissionId = await SeedSubmissionAsync();

        var response = await _client.GetAsync($"/submissions/{submissionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SubmissionDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(submissionId);
        result.CurrentStatus.Should().Be("Received");
    }

    [Fact]
    public async Task GetSubmission_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/submissions/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubmissionTransitions_ReturnsSeededTransitions()
    {
        var submissionId = await SeedSubmissionAsync();
        await SeedWorkflowTransitionAsync("Submission", submissionId, "Received", "Triaging", "seed");

        var response = await _client.GetAsync($"/submissions/{submissionId}/transitions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<WorkflowTransitionRecordDto>>();
        result.Should().ContainSingle();
        result![0].ToState.Should().Be("Triaging");
    }

    [Fact]
    public async Task PostSubmissionTransition_Valid_Returns201AndPersists()
    {
        var submissionId = await SeedSubmissionAsync();

        var response = await _client.PostAsJsonAsync(
            $"/submissions/{submissionId}/transitions",
            new WorkflowTransitionRequestDto("Triaging", "triage"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<WorkflowTransitionRecordDto>();
        result.Should().NotBeNull();
        result!.FromState.Should().Be("Received");
        result.ToState.Should().Be("Triaging");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Submissions.Single(s => s.Id == submissionId).CurrentStatus.Should().Be("Triaging");
    }

    [Fact]
    public async Task PostSubmissionTransition_Invalid_Returns409()
    {
        var submissionId = await SeedSubmissionAsync();

        var response = await _client.PostAsJsonAsync(
            $"/submissions/{submissionId}/transitions",
            new WorkflowTransitionRequestDto("Binding", "skip"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PostSubmissionTransition_MissingToState_Returns400()
    {
        var submissionId = await SeedSubmissionAsync();

        var response = await _client.PostAsJsonAsync(
            $"/submissions/{submissionId}/transitions",
            new WorkflowTransitionRequestDto(string.Empty, "missing"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRenewal_Existing_Returns200()
    {
        var renewalId = await SeedRenewalAsync();

        var response = await _client.GetAsync($"/renewals/{renewalId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RenewalDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(renewalId);
        result.CurrentStatus.Should().Be("Created");
    }

    [Fact]
    public async Task GetRenewalTransitions_ReturnsSeededTransitions()
    {
        var renewalId = await SeedRenewalAsync();
        await SeedWorkflowTransitionAsync("Renewal", renewalId, "Created", "DataReview", "seed");

        var response = await _client.GetAsync($"/renewals/{renewalId}/transitions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<WorkflowTransitionRecordDto>>();
        result.Should().ContainSingle();
        result![0].ToState.Should().Be("DataReview");
    }

    [Fact]
    public async Task PostRenewalTransition_Valid_Returns201AndPersists()
    {
        var renewalId = await SeedRenewalAsync();

        var response = await _client.PostAsJsonAsync(
            $"/renewals/{renewalId}/transitions",
            new WorkflowTransitionRequestDto("DataReview", "start"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<WorkflowTransitionRecordDto>();
        result.Should().NotBeNull();
        result!.FromState.Should().Be("Created");
        result.ToState.Should().Be("DataReview");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Renewals.Single(r => r.Id == renewalId).CurrentStatus.Should().Be("DataReview");
    }

    [Fact]
    public async Task PostRenewalTransition_NotFound_Returns404()
    {
        var response = await _client.PostAsJsonAsync(
            $"/renewals/{Guid.NewGuid()}/transitions",
            new WorkflowTransitionRequestDto("DataReview", "missing"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostRenewalTransition_Invalid_Returns409()
    {
        var renewalId = await SeedRenewalAsync();

        var response = await _client.PostAsJsonAsync(
            $"/renewals/{renewalId}/transitions",
            new WorkflowTransitionRequestDto("Negotiation", "skip"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<Guid> SeedSubmissionAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var account = new Account
        {
            Name = $"Submission Account {Guid.NewGuid():N}",
            Industry = "Technology",
            PrimaryState = "CA",
            Region = "West",
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };
        var broker = new Broker
        {
            LegalName = $"Submission Broker {Guid.NewGuid():N}",
            LicenseNumber = $"SUB-{Guid.NewGuid().ToString("N")[..8]}",
            State = "CA",
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };
        var submission = new Submission
        {
            Account = account,
            Broker = broker,
            CurrentStatus = "Received",
            LineOfBusiness = "Cyber",
            EffectiveDate = now.Date.AddDays(30),
            PremiumEstimate = 25000m,
            AssignedToUserId = userId,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };

        db.Submissions.Add(submission);
        await db.SaveChangesAsync();
        return submission.Id;
    }

    private async Task<Guid> SeedRenewalAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var account = new Account
        {
            Name = $"Renewal Account {Guid.NewGuid():N}",
            Industry = "Healthcare",
            PrimaryState = "NY",
            Region = "East",
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };
        var broker = new Broker
        {
            LegalName = $"Renewal Broker {Guid.NewGuid():N}",
            LicenseNumber = $"REN-{Guid.NewGuid().ToString("N")[..8]}",
            State = "NY",
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };
        var renewal = new Renewal
        {
            Account = account,
            Broker = broker,
            CurrentStatus = "Created",
            LineOfBusiness = "Property",
            RenewalDate = now.Date.AddDays(45),
            AssignedToUserId = userId,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
        };

        db.Renewals.Add(renewal);
        await db.SaveChangesAsync();
        return renewal.Id;
    }

    private async Task SeedWorkflowTransitionAsync(string workflowType, Guid entityId, string fromState, string toState, string reason)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.WorkflowTransitions.Add(new WorkflowTransition
        {
            WorkflowType = workflowType,
            EntityId = entityId,
            FromState = fromState,
            ToState = toState,
            Reason = reason,
            ActorUserId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
    }
}
