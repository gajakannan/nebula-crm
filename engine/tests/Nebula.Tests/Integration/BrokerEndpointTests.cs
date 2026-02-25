using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Nebula.Application.DTOs;

namespace Nebula.Tests.Integration;

public class BrokerEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateBroker_WithValidData_Returns201()
    {
        var dto = new BrokerCreateDto("Test Broker LLC", "TEST-LIC-001", "CA", "test@broker.com", "+14155551234");

        var response = await _client.PostAsJsonAsync("/api/brokers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<BrokerDto>();
        result.Should().NotBeNull();
        result!.LegalName.Should().Be("Test Broker LLC");
        result.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task CreateBroker_DuplicateLicense_Returns409()
    {
        var dto = new BrokerCreateDto("First Broker", "DUP-LIC-001", "NY", null, null);
        await _client.PostAsJsonAsync("/api/brokers", dto);

        var dto2 = new BrokerCreateDto("Second Broker", "DUP-LIC-001", "CA", null, null);
        var response = await _client.PostAsJsonAsync("/api/brokers", dto2);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ListBrokers_ReturnsPagedResult()
    {
        await _client.PostAsJsonAsync("/api/brokers",
            new BrokerCreateDto("Listed Broker", "LIST-001", "TX", null, null));

        var response = await _client.GetAsync("/api/brokers?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonPaginatedBrokerList>();
        json.Should().NotBeNull();
        json!.Data.Should().NotBeEmpty();
        json.TotalCount.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetBroker_ExistingId_Returns200()
    {
        var create = await _client.PostAsJsonAsync("/api/brokers",
            new BrokerCreateDto("Get Broker", "GET-001", "WA", null, null));
        var created = await create.Content.ReadFromJsonAsync<BrokerDto>();

        var response = await _client.GetAsync($"/api/brokers/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBroker_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/brokers/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBroker_WithIfMatch_Returns200()
    {
        var create = await _client.PostAsJsonAsync("/api/brokers",
            new BrokerCreateDto("Update Broker", "UPD-001", "OR", null, null));
        var created = await create.Content.ReadFromJsonAsync<BrokerDto>();

        var updateDto = new BrokerUpdateDto("Updated Broker Name", "WA", "Active", "new@email.com", null);
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/brokers/{created!.Id}")
        {
            Content = JsonContent.Create(updateDto),
        };
        request.Headers.IfMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue($"\"{created.RowVersion}\""));

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteBroker_ExistingBroker_Returns204()
    {
        var create = await _client.PostAsJsonAsync("/api/brokers",
            new BrokerCreateDto("Delete Broker", "DEL-001", "NV", null, null));
        var created = await create.Content.ReadFromJsonAsync<BrokerDto>();

        var response = await _client.DeleteAsync($"/api/brokers/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateBroker_InvalidState_Returns400()
    {
        var dto = new BrokerCreateDto("Bad Broker", "BAD-001", "INVALID", null, null);
        var response = await _client.PostAsJsonAsync("/api/brokers", dto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record JsonPaginatedBrokerList(
        IReadOnlyList<BrokerDto> Data, int Page, int PageSize, int TotalCount, int TotalPages);
}
