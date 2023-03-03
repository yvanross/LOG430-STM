namespace STM.Entities.DTO;

public class TrackingBusDTO
{
    /// <summary>
    /// Number of the bus (aka 107)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// TripID of the bus
    /// </summary>
    public string BusID { get; set; }

    /// <summary>
    /// TripID of the bus
    /// </summary>
    public string TripID { get; set; }

    /// <summary>
    /// Estimated Time of Arrival
    /// </summary>
    public string ETA { get; set; }

    /// <summary>
    /// First stop of the bus trip
    /// </summary>
    public StopScheduleDTO? OriginStopSchedule { get; set; }

    /// <summary>
    /// Last stop of the bus trip
    /// </summary>
    public StopScheduleDTO? TargetStopSchedule { get; set; }

    /// <summary>
    /// First stop of the bus trip
    /// </summary>
    public int indexOfOriginStop { get; set; }

    /// <summary>
    /// Last stop of the bus trip
    /// </summary>
    public int indexOfDestinationStop { get; set; }

    public string callBack { get; set; }

    public string processID { get; set; }
}