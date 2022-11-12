namespace OrderingService.Domain.Events;

public class OrderCreatedEvent : OrderDomainEvent
{
    public Order Order { get; }
    public OrderCreatedEvent(Guid orderId, Order order) 
        : base(Guid.NewGuid(), orderId, nameof(OrderCreatedEvent))
    {
        Order = order;
    }
}
