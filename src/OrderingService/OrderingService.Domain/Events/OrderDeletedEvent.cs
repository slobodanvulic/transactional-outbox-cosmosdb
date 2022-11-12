namespace OrderingService.Domain.Events;

public class OrderDeletedEvent : OrderDomainEvent
{
    public OrderDeletedEvent(Guid orderId) : base(Guid.NewGuid(), orderId, nameof(OrderDeletedEvent))
    {
    }
}
