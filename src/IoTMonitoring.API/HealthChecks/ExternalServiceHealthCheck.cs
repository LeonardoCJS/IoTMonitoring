using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IoTMonitoring.API.HealthChecks;

public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var url = _configuration["HealthChecks:ExternalServiceUrl"];
        if (string.IsNullOrWhiteSpace(url))
        {
            return HealthCheckResult.Degraded("ExternalServiceUrl not configured.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("external-health");
            using var response = await client.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External service is reachable.");
            }

            return HealthCheckResult.Unhealthy($"External service returned status code {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("External service is unavailable.", ex);
        }
    }
}
