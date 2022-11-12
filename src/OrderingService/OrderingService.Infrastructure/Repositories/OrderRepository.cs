using OrderingService.Application;
using OrderingService.Domain;
using OrderingService.Infrastructure.Context;

namespace OrderingService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IContainerContext _context;
    private readonly IPartitionKeyProvider _partitionKeyProvider;

    public OrderRepository(IContainerContext context, IPartitionKeyProvider partitionKeyProvider)
    {
        _context = context;
        _partitionKeyProvider = partitionKeyProvider;
    }

    public void Create(Order order)
    {
        var data = new DataObject<Order>()
        {
            Id = order.Id.ToString(),
            PartitionKey = _partitionKeyProvider.GetPartitionKey(order),
            Type = TypeConstants.Order,
            Data = order,
            State = EntityState.Created
        };

        _context.Add(data);
    }

    public Task DeleteAsync(Guid id, string etag)
    {
        throw new NotImplementedException();
    }

    public Task<(List<(Order, string)>, bool, string)> ReadAllAsync(int pageSize, string continuationToken)
    {
        throw new NotImplementedException();
    }

    public Task<(Order, string)> ReadAsync(Guid id, string etag)
    {
        throw new NotImplementedException();
    }

    public void Update(Order order, string etag)
    {
        throw new NotImplementedException();
    }

    public Task<List<IDataObject<Entity>>> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
