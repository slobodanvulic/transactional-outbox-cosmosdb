using System.Net;
using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OrderingService.Application.Commands.Handlers;
using OrderingService.Application.Model;

namespace OrderingService.Api.Triggers;

public class CreateOrderApi
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public CreateOrderApi(ILoggerFactory loggerFactory, IMediator mediator)
    {
        _logger = loggerFactory.CreateLogger<CreateOrderApi>();
        _mediator = mediator;
    }

    [Function(nameof(CreateOrderApi))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order")] 
    HttpRequestData req, CancellationToken cancellationToken)
    {
        CreateOrderDto? createOrderDto;
        HttpResponseData response;

        try
        {
            createOrderDto = await JsonSerializer.DeserializeAsync<CreateOrderDto>(req.Body, cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            _logger.LogError("Deserialization error");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var result = await _mediator.Send(new CreateOrderCommand(createOrderDto!), cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("[OrderingService] Order created.");
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            return response;
        }

        var details = string.Join(Environment.NewLine, result.Errors.Select(e => e.Message).ToArray());
        _logger.LogError("[OrderingService] Creating order failed. Details: {details}", details);

        //should check for error type and return proper status code
        response = req.CreateResponse(HttpStatusCode.BadRequest);
        response.Headers.Add("Content-Type", "application/problem+json; charset=utf-8");
        var propblemDetails = new ProblemDetails()
        {
            Detail = details
        };
        response.Body = new MemoryStream(Encoding.Default.GetBytes(JsonSerializer.Serialize(propblemDetails)));
        return response;
    }
}
