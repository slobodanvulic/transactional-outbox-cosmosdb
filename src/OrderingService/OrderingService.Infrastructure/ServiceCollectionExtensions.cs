using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using OrderingService.Application;
using OrderingService.Domain.Events;
using OrderingService.Infrastructure.Context;
using OrderingService.Infrastructure.Repositories;

namespace OrderingService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, CosmosDbConfiguration cosmosDbConfiguration)
    {
        services
            .AddCosmosDb(cosmosDbConfiguration)
            .AddSingleton<IPartitionKeyProvider>(new PartitionKeyProvider())
            .AddScoped<IContainerContext, CosmosContainerContext>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services, CosmosDbConfiguration cosmosDbConfiguration)
    {
        var cosmosOpts = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            }
        };

        var containers = new List<(string, string)>
        {
            (cosmosDbConfiguration.CosmosDbDatabaseName, cosmosDbConfiguration.CosmosDbContainerName)
        };

        var cosmosClient = CosmosClient.CreateAndInitializeAsync(cosmosDbConfiguration.CosmosDbConnectionString, containers, cosmosOpts).Result;
        var container = cosmosClient.GetContainer(cosmosDbConfiguration.CosmosDbDatabaseName, cosmosDbConfiguration.CosmosDbContainerName);
        
        return services.AddSingleton(container);
    }
}
