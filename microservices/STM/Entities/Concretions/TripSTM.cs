using Entities.Domain;

namespace Entities.Concretions;

public class TripSTM : ITripSTM
{
    public string Id { get; init; }
    public List<StopScheduleSTM> StopSchedules { get; set; } = new ();

    public string RelevantOriginStopId { get; set; }
  
    public string RelevantDestinationStopId { get; set; }

    public StopScheduleSTM? RelevantOrigin { get; set; }

    public StopScheduleSTM? RelevantDestination { get; set; }
    public bool FromStaticGtfs { get; set; } = true;
}