using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OrderingService.Domain;
using System.Dynamic;
using System.Text.Json;

namespace OrderingService.Api.Triggers;

public class EventHandler
{
    private readonly ILogger _logger;

    public EventHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventHandler>();
    }

    [Function(nameof(EventHandler))]
    [ServiceBusOutput("%ServiceBusTopicName%", ServiceBusEntityType.Topic, Connection = "ServiceBusConnectionString")]
    public IEnumerable<string> Run([CosmosDBTrigger(
        databaseName: "%CosmosDbDatabaseName%",
        collectionName: "%CosmosDbContainerName%",
        ConnectionStringSetting = "CosmosDbConnectionString",
        LeaseCollectionName = "%CosmosDbLeasesContainerName%")] IReadOnlyList<ExpandoObject> changes)
    {
        foreach (var document in changes as dynamic)
        {
            if (!IsDomainEvent(document))
            {
                continue;
            }
            yield return JsonSerializer.Serialize(document.data);
        }
    }

    private bool IsDomainEvent(dynamic document)
    {
        if (((IDictionary<string, object>)document).ContainsKey("type")
                && ((IDictionary<string, object>)document).ContainsKey("data")
                && document.type.ToString() is TypeConstants.DomainEvent)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This method can be used with <see cref="ServiceBusClient"/>
    /// <see cref="ServiceBusOutputAttribute"/> does not suport AMQP messages (<see cref="ServiceBusMessage"/>)
    /// </summary>
    private static void CreateServiceBusMessage(dynamic document, string jsonMessage)
    {
        var sbMessage = new ServiceBusMessage(jsonMessage)
        {
            ContentType = "application/json",
            Subject = document.data.GetProperty("action").ToString(),
            MessageId = document.id.ToString(),
            PartitionKey = document.partitionKey.ToString(),
            SessionId = document.partitionKey.ToString()
        };
    }
}
