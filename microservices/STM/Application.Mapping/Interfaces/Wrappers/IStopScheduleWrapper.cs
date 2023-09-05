namespace Application.Mapping.Interfaces.Wrappers;

public interface IStopScheduleWrapper
{
    string StopId { get; }

    TimeSpan DepartureTime { get; }
}