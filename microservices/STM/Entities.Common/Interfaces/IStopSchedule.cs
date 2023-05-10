namespace Entities.Common.Interfaces;

public interface IStopSchedule
{
    IStop Stop { get; set; }

    DateTime DepartureTime { get; set; }
}