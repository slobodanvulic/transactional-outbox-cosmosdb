using OrderingService.Application;
using OrderingService.Domain;
using OrderingService.Domain.Events;
using OrderingService.Infrastructure.Context;

namespace OrderingService.Infrastructure.Repositories;

public class EventRepository: IEventRepository
{
    private readonly IContainerContext _context;
    private readonly IPartitionKeyProvider _partitionKeyProvider;
    private readonly int _timeToLive;

    public EventRepository(IContainerContext context, IPartitionKeyProvider partitionKeyProvider, int timeToLive = -1)
    {
        _partitionKeyProvider = partitionKeyProvider;
        _context = context;
        _timeToLive = timeToLive;
    }

    public void Create(OrderDomainEvent domainevent)
    {
        var data = new DataObject<OrderDomainEvent>()
        {
            Id = domainevent.Id.ToString(),
            PartitionKey = _partitionKeyProvider.GetPartitionKey(domainevent),
            Type = TypeConstants.DomainEvent,
            Data = domainevent,
            State = EntityState.Created
        };

        _context.Add(data);
    }
}
