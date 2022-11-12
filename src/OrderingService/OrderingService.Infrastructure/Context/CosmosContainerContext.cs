using MediatR;
using Microsoft.Azure.Cosmos;
using OrderingService.Application;
using OrderingService.Domain;
using OrderingService.Domain.Events;
using System.Net;

namespace OrderingService.Infrastructure.Context;

public class CosmosContainerContext : IContainerContext
{
    private readonly IMediator _mediator;
    private readonly Container _container;
    private readonly List<IDataObject<Entity>> _dataObjects; // TODO: should be thread safe data structure

    public CosmosContainerContext(Container container, IMediator mediator)
    {
        _container = container;
        _mediator = mediator;
        _dataObjects = new();
    }

    public void Add(IDataObject<Entity> entity)
    {
        if (_dataObjects.FindIndex(0,o => o.Id == entity.Id && o.PartitionKey == entity.PartitionKey) == -1)
        {
            _dataObjects.Add(entity);
        }
    }

    public void Reset()
    {
        _dataObjects.Clear();
    }

    public async Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await RaiseDomainEventsAsync(_dataObjects, cancellationToken);
        switch (_dataObjects.Count)
        {
            case 1:
                {
                    var result = await SaveSingleAsync(_dataObjects[0], cancellationToken);
                    return result;
                }
            case > 1:
                {
                    var result = await SaveInTransactionalBatchAsync(cancellationToken);
                    return result;
                }
            default:
                return new List<IDataObject<Entity>>();
        }
    }

    private async Task<List<IDataObject<Entity>>> SaveSingleAsync(IDataObject<Entity> dObj, CancellationToken cancellationToken)
    {
        var reqOptions = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false
        };

        if (!string.IsNullOrWhiteSpace(dObj.Etag)) reqOptions.IfMatchEtag = dObj.Etag;

        var pk = new PartitionKey(dObj.PartitionKey);

        try
        {
            ItemResponse<IDataObject<Entity>> response;

            switch (dObj.State)
            {
                case EntityState.Created:
                    response = await _container.CreateItemAsync(dObj, pk, reqOptions, cancellationToken);
                    break;
                case EntityState.Updated:
                case EntityState.Deleted:
                    response = await _container.ReplaceItemAsync(dObj, dObj.Id, pk, reqOptions, cancellationToken);
                    break;
                default:
                    _dataObjects.Clear();
                    return new List<IDataObject<Entity>>();
            }

            dObj.Etag = response.ETag;
            var result = new List<IDataObject<Entity>>(1) { dObj };

            // work has been successfully done - reset DataObjects list
            _dataObjects.Clear();
            return result;
        }
        catch (CosmosException e)
        {
            // Not recoverable - clear context
            _dataObjects.Clear();
            throw EvaluateCosmosError(e.StatusCode, Guid.Parse(dObj.Id), dObj.Etag);
        }
    }

    private async Task<List<IDataObject<Entity>>> SaveInTransactionalBatchAsync(CancellationToken cancellationToken)
    {
        if (_dataObjects.Count > 0)
        {
            var pk = new PartitionKey(_dataObjects[0].PartitionKey);
            var tb = _container.CreateTransactionalBatch(pk);
            _dataObjects.ForEach(o =>
            {
                TransactionalBatchItemRequestOptions tro = null;

                if (!string.IsNullOrWhiteSpace(o.Etag))
                    tro = new TransactionalBatchItemRequestOptions { IfMatchEtag = o.Etag };

                switch (o.State)
                {
                    case EntityState.Created:
                        tb.CreateItem(o);
                        break;
                    case EntityState.Updated or EntityState.Deleted:
                        tb.ReplaceItem(o.Id, o, tro);
                        break;
                }
            });

            var tbResult = await tb.ExecuteAsync(cancellationToken);

            if (!tbResult.IsSuccessStatusCode)
                for (var i = 0; i < _dataObjects.Count; i++)
                    if (tbResult[i].StatusCode != HttpStatusCode.FailedDependency)
                    {
                        // Not recoverable - clear context
                        _dataObjects.Clear();
                        throw EvaluateCosmosError(tbResult[i].StatusCode);
                    }

            for (var i = 0; i < _dataObjects.Count; i++)
                _dataObjects[i].Etag = tbResult[i].ETag;
        }

        var result = new List<IDataObject<Entity>>(_dataObjects); // return copy of list as result

        // work has been successfully done - reset DataObjects list
        _dataObjects.Clear();
        return result;
    }

    private async Task RaiseDomainEventsAsync(List<IDataObject<Entity>> dataObjects, CancellationToken cancelationToken)
    {
        var eventEmitters = new List<IEventEmitter<IEvent>>();

        foreach (var o in dataObjects)
            if (o.Data is IEventEmitter<IEvent> ee)
                eventEmitters.Add(ee);

        if (eventEmitters.Count <= 0) 
            return;

        foreach (var evt in eventEmitters.SelectMany(eventEmitter => eventEmitter.DomainEvents))
            await _mediator.Publish(evt, cancelationToken); 
    }

    private Exception EvaluateCosmosError(HttpStatusCode statusCode, Guid? id = null, string etag = null)
    {
        //TODO: implement different types of exceptions
        return statusCode switch
        {
            HttpStatusCode.NotFound => new Exception(
                $"Domain object not found for Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
            HttpStatusCode.NotModified => new Exception(
                $"Domain object not modified. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
            HttpStatusCode.Conflict => new Exception(
                $"Domain object conflict detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
            HttpStatusCode.PreconditionFailed => new Exception(
                $"Domain object mid-air collision detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
            HttpStatusCode.TooManyRequests => new Exception(
                "Too many requests occurred. Try again later)"),
            _ => new Exception("Cosmos Exception")
        };
    }
}
