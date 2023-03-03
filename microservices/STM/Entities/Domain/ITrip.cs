namespace Entities.Domain;

public interface ITrip : ICloneable
{
    public string Id { get; init; }
    
    public List<IStopSchedule> StopSchedules { get; }

    public bool FromStaticGtfs { get; init; }
}