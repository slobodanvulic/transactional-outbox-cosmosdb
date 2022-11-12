using Microsoft.Azure.Cosmos;
using OrderingService.Application;
using OrderingService.Domain;

namespace OrderingService.Infrastructure.Context;

public interface IContainerContext
{
    public void Add(IDataObject<Entity> entity);
    public Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default);
    public void Reset();
}
