
namespace Entities.Domain;

public interface IStopSchedule
{
    IStopSTM Stop { get; set; }

    DateTime DepartureTime { get; set; }
}