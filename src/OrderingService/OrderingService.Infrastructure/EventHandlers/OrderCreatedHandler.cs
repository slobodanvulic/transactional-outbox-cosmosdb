using MediatR;
using OrderingService.Domain.Events;

namespace OrderingService.Infrastructure.EventHandlers;

public class OrderCreatedHandler : INotificationHandler<OrderCreatedEvent>
{
    private IEventRepository _eventRepository;

    public OrderCreatedHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _eventRepository.Create(notification);
        return Task.CompletedTask;
    }
}
