using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderingService.Application.Commands.Handlers;
using OrderingService.Domain;
using OrderingService.Infrastructure;
using OrderingService.Infrastructure.EventHandlers;
using System.Reflection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(app => app.AddUserSecrets(Assembly.GetExecutingAssembly(), true))
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddApplicationInsights();
    })
    .ConfigureServices((context, services) =>
    {
        services
        .AddInfrastructure(context.Configuration.Get<CosmosDbConfiguration>())
        .AddMediatR(typeof(Order), typeof(CreateOrderCommandHandler), typeof(OrderCreatedHandler));


        var serviceBusConnection = context.Configuration["ServiceBusConnectionString"];
        var serviceBusTopic = context.Configuration["ServiceBusTopicName"];
        var cosmosDbConnection = context.Configuration["CosmosDbConnectionString"];
        services
        .AddHealthChecks()
        .AddAzureServiceBusTopic(serviceBusConnection, serviceBusTopic)
        .AddCosmosDb(cosmosDbConnection);

    })
    .Build();

host.Run();
