using Entities.Domain;

namespace StaticGTFS.Concretions;

public class Trip : ITrip
{
    public string Id { get; init; }
    public List<IStopSchedule> StopSchedules { get; internal set; } = new ();
    public object Clone()
    {
        return new Trip()
        {
            Id = Id,
            StopSchedules = StopSchedules.ConvertAll(sS => (IStopSchedule)sS.Clone())
        };
    }
}