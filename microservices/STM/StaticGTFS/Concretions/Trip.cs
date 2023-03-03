using Entities.Domain;

namespace StaticGTFS.Concretions;

public class Trip : ITrip
{
    public string ID { get; init; }
    public List<IStopSchedule> StopSchedules { get; internal set; } = new ();
}