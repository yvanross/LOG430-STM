// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace

using Application.EventHandlers;

namespace Contracts;

public class StaticGtfsDataLoaded : Event
{
    public StaticGtfsDataLoaded(Guid id, DateTime created) : base(id, created)
    {
    }
}