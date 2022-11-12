namespace OrderingService.Domain.Events;

public class OrderDescriptionUpdatedEvent : OrderDomainEvent
{
    public string Description { get; }

    public OrderDescriptionUpdatedEvent(Guid orderId, string description) 
        : base(Guid.NewGuid(), orderId, nameof(OrderDescriptionUpdatedEvent))
    {
        Description = description;
    }
}
