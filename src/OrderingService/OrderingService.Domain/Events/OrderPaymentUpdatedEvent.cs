using OrderingService.Domain.ValueObjects;

namespace OrderingService.Domain.Events;

public class OrderPaymentUpdatedEvent : OrderDomainEvent
{
    public Payment Payment { get; }
    public OrderPaymentUpdatedEvent(Guid orderId, Payment payment) 
        : base(Guid.NewGuid(), orderId, nameof(OrderPaymentUpdatedEvent))
    {
        Payment = payment;
    }
}
