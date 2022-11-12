using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace OrderingService.Api.Triggers;

/// <summary>
/// Health check on Function App is not available for Consumption Plan
/// </summary>
public class HealthCheckApi
{
    private readonly ILogger _logger;
    private readonly HealthCheckService _healthCheck;

    public HealthCheckApi(ILoggerFactory loggerFactory, HealthCheckService healthCheck)
    {
        _logger = loggerFactory.CreateLogger<HealthCheckApi>();
        _healthCheck = healthCheck;
    }

    [Function(nameof(HealthCheckApi))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="health")] 
    HttpRequestData req)
    {
        var status = await _healthCheck.CheckHealthAsync();

        _logger.LogInformation("Health check status: {status}", status.Status);

        var response = status.Status == HealthStatus.Healthy ? HttpStatusCode.OK : HttpStatusCode.FailedDependency;

        return req.CreateResponse(response);
    }
}
