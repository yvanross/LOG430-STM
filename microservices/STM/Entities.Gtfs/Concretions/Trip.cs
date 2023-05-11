using Entities.Common.Interfaces;
using Entities.Gtfs.Interfaces;

namespace Entities.Gtfs.Concretions;

public class Trip : ITrip
{
    public string Id { get; init; }
   
    public List<IStopSchedule> StopSchedules { get; set; } = new ();
}