using OrderingService.Domain;
using OrderingService.Domain.Events;

namespace OrderingService.Infrastructure.Repositories;

public interface IPartitionKeyProvider
{
    public string GetPartitionKey(Order order);
    public string GetPartitionKey(OrderDomainEvent orderDomainEvent);
}
