using Entities.Domain;
using STM.Entities.Concretions;

namespace STM.Entities.Domain;

public interface ITripSTM
{
    public string ID { get; init; }
    
    public List<StopScheduleSTM> StopSchedules { get; set; }

    public string RelevantOriginStopId { get; set; }

    public string RelevantDestinationStopId { get; set; }

    public StopScheduleSTM? RelevantOrigin { get; set; }

    public StopScheduleSTM? RelevantDestination { get; set; }

    public bool FromStaticGtfs { get; set; }
}