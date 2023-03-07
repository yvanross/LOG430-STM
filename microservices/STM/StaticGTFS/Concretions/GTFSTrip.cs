using Entities.Domain;
using GTFS.Interfaces;

namespace GTFS.Concretions;

public class GTFSTrip : IGTFSTrip
{
    public string Id { get; init; }
   
    public List<IStopSchedule> StopSchedules { get; set; } = new ();
}