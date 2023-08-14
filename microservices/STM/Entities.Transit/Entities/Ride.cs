using Entities.Gtfs.Concretions;

namespace Entities.Transit.Entities;

public class Ride
{
    public required string Id { get; init; }

    public Trip Trip
    {
        get => _trip;
        private set => _trip = value;
    }

    public StopSchedule Origin { get; private set; }

    public StopSchedule Destination { get; private set; }


    private Trip _trip;
}