using Entities.Transit.Concretions;

namespace Entities.Transit.Interfaces;

public interface ITransitTrip
{
    public string Id { get; init; }
    
    public List<TransitStopSchedule> StopSchedules { get; set; }

    public string RelevantOriginStopId { get; set; }

    public string RelevantDestinationStopId { get; set; }

    public TransitStopSchedule? RelevantOrigin { get; set; }

    public TransitStopSchedule? RelevantDestination { get; set; }
}