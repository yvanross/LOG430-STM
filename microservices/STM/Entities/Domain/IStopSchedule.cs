
namespace Entities.Domain;

public interface IStopSchedule
{
    IStop Stop { get; }

    DateTime DepartureTime { get; }
}