namespace Application.Mapping.Interfaces.Wrappers;

public interface IStopScheduleWrapper
{
    string StopId { get; }

    DateTime DepartureTime { get; }
}