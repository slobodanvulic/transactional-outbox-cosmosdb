using Newtonsoft.Json;
using OrderingService.Application;
using OrderingService.Domain;

namespace OrderingService.Infrastructure.Context;

public class DataObject<T> : IDataObject<T> where T : Entity
{
    [JsonProperty] 
    public string Id { get; set; }
    [JsonProperty] 
    public string PartitionKey { get; set; }
    [JsonProperty] 
    public string Type { get; set; }
    public T Data { get; set; }
    [JsonProperty("_etag")] 
    public string Etag { get; set; }
    public int Ttl { get; } = -1;

    [JsonIgnore] 
    public EntityState State { get; set; }

}
