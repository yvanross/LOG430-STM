namespace Application.Mapping.Interfaces.Wrappers;

public interface ITripWrapper : IDisposable
{
    string TripId { get; }

    List<IStopScheduleWrapper> ScheduledStops { get; }
}