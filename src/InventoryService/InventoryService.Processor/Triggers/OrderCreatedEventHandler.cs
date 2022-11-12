using InventoryService.Processor.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InventoryService.Processor.Triggers;

public class OrderCreatedEventHandler
{
    private readonly ILogger _logger;

    public OrderCreatedEventHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<OrderCreatedEventHandler>();
    }

    [Function(nameof(OrderCreatedEventHandler))]
    public void Run([ServiceBusTrigger("%ServiceBusigOrderCreatedQueueName%",
        Connection = "ServiceBusConnectionString")] string orderCreatedEventJson)
    {
        OrderCreatedEvent? orderCreatedEvent;
        try
        {
            orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>
            (orderCreatedEventJson, JsonSerializerConfiguration.Default);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization error, event is not of type OrderCreatedEvent.");
            return; //skip processing 
        }

        _logger.LogInformation($"[InventoryService] OrderCreatedEvent event received. " +
            $"Created at: {orderCreatedEvent!.CreatedAt}. Order details: {orderCreatedEvent.Order}");
    }

}
