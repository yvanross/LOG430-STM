using Entities.Domain;

namespace GTFS.Interfaces;

public interface IGTFSTrip : ICloneable
{
    public string Id { get; init; }

    public List<IStopSchedule> StopSchedules { get; }

    public bool FromStaticGtfs { get; init; }
}