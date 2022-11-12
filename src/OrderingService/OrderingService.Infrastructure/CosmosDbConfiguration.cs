namespace OrderingService.Infrastructure;

public class CosmosDbConfiguration
{
    public string CosmosDbConnectionString { get; set; }
    public string CosmosDbDatabaseName { get; set; }
    public string CosmosDbContainerName { get; set; }
}
