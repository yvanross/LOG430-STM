using Contracts;

namespace Application.EventHandlers.Handlers;

public class StaticGtfsDataUpdatedEventHandler : IApplicationEventHandler<StaticGtfsDataUpdated>
{
    private readonly IEventContext _eventContext;

    public StaticGtfsDataUpdatedEventHandler(IEventContext eventContext)
    {
        _eventContext = eventContext;
    }
    public async Task HandleAsync(StaticGtfsDataUpdated applicationEvent)
    {
        await _eventContext.AddOrUpdateAsync(applicationEvent);
    }
}