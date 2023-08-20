using Domain.Aggregates.Bus;

namespace Domain.Services.Aggregates;

public class BusServices
{
    public Bus CreateBus(string id, string name, string tripId, int currentStopIndex)
    {
        return new Bus(id, name, tripId, currentStopIndex);
    }
}