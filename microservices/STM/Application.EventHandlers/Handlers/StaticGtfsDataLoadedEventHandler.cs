using Application.EventHandlers.Interfaces;
using Contracts;

namespace Application.EventHandlers.Handlers;

public class StaticGtfsDataLoadedEventHandler : IApplicationEventHandler<StaticGtfsDataLoaded>
{
    private readonly IEventContext _eventContext;

    public StaticGtfsDataLoadedEventHandler(IEventContext eventContext)
    {
        _eventContext = eventContext;
    }
    public async Task HandleAsync(StaticGtfsDataLoaded applicationEvent)
    {
        await _eventContext.AddOrUpdateAsync(applicationEvent);
    }
}