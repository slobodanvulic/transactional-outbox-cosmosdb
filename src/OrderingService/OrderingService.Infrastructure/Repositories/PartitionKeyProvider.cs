using OrderingService.Domain;
using OrderingService.Domain.Events;

namespace OrderingService.Infrastructure.Repositories;

public class PartitionKeyProvider : IPartitionKeyProvider
{
    public string GetPartitionKey(Order order)
    {
        return order.Id.ToString();
    }

    public string GetPartitionKey(OrderDomainEvent orderDomainEvent)
    {
        return orderDomainEvent.OrderId.ToString();
    }
}
