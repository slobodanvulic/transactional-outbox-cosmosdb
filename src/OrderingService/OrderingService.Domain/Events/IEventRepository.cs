namespace OrderingService.Domain.Events;

public interface IEventRepository
{
    public void Create(OrderDomainEvent e);
}
