using Newtonsoft.Json;

namespace OrderingService.Domain.Events;

public abstract class OrderDomainEvent : Entity, IEvent
{
    public Guid OrderId { get; }
    public string Action { get; }

    [JsonProperty] 
    public DateTimeOffset CreatedAt { get; }
    
    protected OrderDomainEvent(Guid id, Guid orderId, string action)
    {
        Id = id;
        OrderId = orderId;
        Action = action;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
