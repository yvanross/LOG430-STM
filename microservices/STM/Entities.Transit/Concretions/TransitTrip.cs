using Entities.Transit.Interfaces;

namespace Entities.Transit.Concretions;

public class TransitTrip : ITransitTrip
{
    public string Id { get; init; }
    public List<TransitStopSchedule> StopSchedules { get; set; } = new ();

    public string RelevantOriginStopId { get; set; }
  
    public string RelevantDestinationStopId { get; set; }

    public TransitStopSchedule? RelevantOrigin { get; set; }

    public TransitStopSchedule? RelevantDestination { get; set; }

    public bool FromStaticGtfs { get; set; } = true;
}