using Entities.Common.Interfaces;

namespace Entities.Gtfs.Interfaces;

public interface ITrip
{
    public string Id { get; init; }

    public List<IStopSchedule> StopSchedules { get; set; }
}