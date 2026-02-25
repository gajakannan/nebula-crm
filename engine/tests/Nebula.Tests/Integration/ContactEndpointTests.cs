using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Nebula.Application.DTOs;

namespace Nebula.Tests.Integration;

public class ContactEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<BrokerDto> CreateBrokerAsync(string license)
    {
        var response = await _client.PostAsJsonAsync("/api/brokers",
            new BrokerCreateDto("Contact Test Broker", license, "CA", null, null));
        return (await response.Content.ReadFromJsonAsync<BrokerDto>())!;
    }

    [Fact]
    public async Task CreateContact_WithValidData_Returns201()
    {
        var broker = await CreateBrokerAsync("CTT-001");

        var dto = new ContactCreateDto(broker.Id, "Jane Doe", "jane@example.com", "+14155551111", "Primary");
        var response = await _client.PostAsJsonAsync("/api/contacts", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ContactDto>();
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task ListContacts_FilterByBrokerId_ReturnsFiltered()
    {
        var broker = await CreateBrokerAsync("CTT-002");
        await _client.PostAsJsonAsync("/api/contacts",
            new ContactCreateDto(broker.Id, "Filter Test", "filter@test.com", "+14155552222", null));

        var response = await _client.GetAsync($"/api/contacts?brokerId={broker.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetContact_NonExistent_Returns404()
    {
        var response = await _client.GetAsync($"/api/contacts/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
