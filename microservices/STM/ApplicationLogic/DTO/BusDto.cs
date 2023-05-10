namespace ApplicationLogic.DTO;

public class BusDto
{
    /// <summary>
    /// Number of the bus (aka 107)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// TripId of the bus
    /// </summary>
    public string BusId { get; set; }

    /// <summary>
    /// TripId of the bus
    /// </summary>
    public string TripId { get; set; }

    /// <summary>
    /// Estimated Time of Arrival
    /// </summary>
    public string ETA { get; set; }

    /// <summary>
    /// Estimated Time of Arrival
    /// </summary>
    public int StopIndexAtTimeOfProcessing { get; set; }

    /// <summary>
    /// First stop of the bus trip
    /// </summary>
    public StopScheduleDto? OriginStopSchedule { get; set; }

    /// <summary>
    /// Last stop of the bus trip
    /// </summary>
    public StopScheduleDto? TargetStopSchedule { get; set; }
}