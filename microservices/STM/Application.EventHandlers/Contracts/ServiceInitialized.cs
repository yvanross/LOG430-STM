using Application.EventHandlers;

namespace Contracts;

public class ServiceInitialized : Event
{
    public ServiceInitialized(Guid id, DateTime created) : base(id, created)
    {
    }
}