
namespace Entities.Domain;

public interface IStopSchedule : ICloneable
{
    IStop Stop { get; }

    DateTime DepartureTime { get; }
}