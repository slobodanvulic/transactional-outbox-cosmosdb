using Newtonsoft.Json;

namespace OrderingService.Domain;

public abstract class Entity : IEntity
{
    [JsonProperty] 
    public Guid Id { get; protected init; }
}
