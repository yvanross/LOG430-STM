using Entities.Domain;

namespace GTFS.Interfaces;

public interface IGTFSTrip
{
    public string Id { get; init; }

    public List<IStopSchedule> StopSchedules { get; set; }
}