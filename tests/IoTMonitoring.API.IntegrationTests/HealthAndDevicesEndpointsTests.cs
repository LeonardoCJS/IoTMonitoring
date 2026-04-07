using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace IoTMonitoring.API.IntegrationTests;

public class HealthAndDevicesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthAndDevicesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_QuandoChamado_DeveRetornarSucesso()
    {
        // Arrange
        const string endpoint = "/health";

        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetDevice_QuandoFluxoCompleto_DeveRetornarDispositivoCriado()
    {
        // Arrange
        var payload = new
        {
            DeviceId = "integration-001",
            Name = "Dispositivo Integracao",
            Location = "Lab"
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/devices", payload);
        var created = await createResponse.Content.ReadFromJsonAsync<DeviceResponse>();
        var getResponse = await _client.GetAsync($"/api/devices/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("integration-001", created.DeviceId);
    }

    private sealed class DeviceResponse
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
    }
}
