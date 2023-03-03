
namespace Entities.Domain;

public interface IStopSchedule : ICloneable
{
    IStopSTM Stop { get; set; }

    DateTime DepartureTime { get; set; }
}