using Application.Mapping.Interfaces.Wrappers;

namespace Infrastructure.FileHandlers.StaticGtfs.Wrappers;

public sealed record TripWrapper(string TripId, List<IStopScheduleWrapper> ScheduledStops) : ITripWrapper
{
    public void Dispose()
    {
        ScheduledStops.Clear();
    }
}
