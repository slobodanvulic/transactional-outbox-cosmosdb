using OrderingService.Domain.ValueObjects;

namespace OrderingService.Domain.Events;

public class OrderCustomerUpdatedEvent : OrderDomainEvent
{
    public Customer Customer { get; }
    public OrderCustomerUpdatedEvent(Guid orderId, Customer customer) 
        : base(Guid.NewGuid(), orderId, nameof(OrderCustomerUpdatedEvent))
    {
        Customer = customer;
    }
}
