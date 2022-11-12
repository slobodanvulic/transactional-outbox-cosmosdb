using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddApplicationInsights();

    })
    .Build();

host.Run();
