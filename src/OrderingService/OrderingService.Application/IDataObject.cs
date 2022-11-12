﻿using OrderingService.Domain;

namespace OrderingService.Application;
public interface IDataObject<out T> where T : Entity
{
    string Id { get; }
    string PartitionKey { get; }
    string Type { get; }
    T Data { get; }
    string Etag { get; set; }
    int Ttl { get; }
    EntityState State { get; set; }
}