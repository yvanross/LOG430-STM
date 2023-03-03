using Entities.Domain;
using GTFS.Interfaces;

namespace GTFS.Concretions;

public class GTFSTrip : IGTFSTrip
{
    public string Id { get; init; }
   
    public List<IStopSchedule> StopSchedules { get; set; } = new ();

    public bool FromStaticGtfs { get; init; } = true;

    public object Clone()
    {
        return new GTFSTrip()
        {
            Id = Id,
            StopSchedules = StopSchedules.ConvertAll(sS => (IStopSchedule)sS.Clone()),
            FromStaticGtfs = FromStaticGtfs
        };
    }
}