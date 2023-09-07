using Application.EventHandlers.Interfaces;
using Contracts;

namespace Application.EventHandlers.Handlers;

public class StmTripModificationAppliedEventHandler : IApplicationEventHandler<StmTripModificationApplied>
{
    private readonly IEventContext _eventContext;

    public StmTripModificationAppliedEventHandler(IEventContext eventContext)
    {
        _eventContext = eventContext;
    }

    public async Task HandleAsync(StmTripModificationApplied applicationEvent)
    {
        await _eventContext.AddOrUpdateAsync(applicationEvent);
    }
}