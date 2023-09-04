// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace

using Application.EventHandlers;

namespace Contracts;

public class BusPositionsUpdated : Event
{
    public BusPositionsUpdated(Guid id, DateTime created) : base(id, created)
    {
    }
}