using OrderingService.Domain;

namespace OrderingService.Application;

public interface IOrderRepository
{
    public void Create(Order order);
    public Task<(Order, string)> ReadAsync(Guid id, string etag);
    public Task DeleteAsync(Guid id, string etag);
    public Task<(List<(Order, string)>, bool, string)> ReadAllAsync(int pageSize, string continuationToken);
    public void Update(Order order, string etag);
    Task<List<IDataObject<Entity>>> CommitAsync(CancellationToken cancellationToken = default);
}
