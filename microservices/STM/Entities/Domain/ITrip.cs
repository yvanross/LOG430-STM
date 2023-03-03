namespace Entities.Domain;

public interface ITrip
{
    public string ID { get; init; }
    
    public List<IStopSchedule> StopSchedules { get; }
}