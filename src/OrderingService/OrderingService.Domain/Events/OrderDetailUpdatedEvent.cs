using OrderingService.Domain.ValueObjects;

namespace OrderingService.Domain.Events;

public class OrderDetailUpdatedEvent : OrderDomainEvent
{
    public OrderDetail OrderDetail { get; }
    public OrderDetailUpdatedEvent(Guid orderId, OrderDetail orderDetail) 
        : base(Guid.NewGuid(), orderId, nameof(OrderDetailUpdatedEvent))
    {
        OrderDetail = orderDetail;
    }
}
