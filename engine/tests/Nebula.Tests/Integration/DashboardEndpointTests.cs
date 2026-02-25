using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Nebula.Application.DTOs;

namespace Nebula.Tests.Integration;

public class DashboardEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetKpis_Returns200WithCorrectShape()
    {
        var response = await _client.GetAsync("/api/dashboard/kpis");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var kpis = await response.Content.ReadFromJsonAsync<DashboardKpisDto>();
        kpis.Should().NotBeNull();
        kpis!.ActiveBrokers.Should().BeGreaterThanOrEqualTo(0);
        kpis.OpenSubmissions.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetPipeline_Returns200WithCorrectShape()
    {
        var response = await _client.GetAsync("/api/dashboard/pipeline");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pipeline = await response.Content.ReadFromJsonAsync<DashboardPipelineDto>();
        pipeline.Should().NotBeNull();
        pipeline!.Submissions.Should().NotBeNull();
        pipeline.Renewals.Should().NotBeNull();
    }

    [Fact]
    public async Task GetNudges_Returns200()
    {
        var response = await _client.GetAsync("/api/dashboard/nudges");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMyTasks_Returns200()
    {
        var response = await _client.GetAsync("/api/my/tasks");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTimelineEvents_Returns200()
    {
        var response = await _client.GetAsync("/api/timeline/events?entityType=Broker");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
