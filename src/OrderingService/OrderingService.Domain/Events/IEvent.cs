using MediatR;

namespace OrderingService.Domain.Events;

public interface IEvent : INotification
{
    public Guid Id { get; }
    public string Action { get; }
}
