using System.Collections.Immutable;

namespace Application.Mapping.Interfaces.Wrappers;

public interface ITripWrapper
{
    string TripId { get; }

    Lazy<ImmutableList<IStopScheduleWrapper>> ScheduledStops { get; }
}