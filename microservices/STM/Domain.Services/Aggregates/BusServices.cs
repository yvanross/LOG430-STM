using Domain.Aggregates.Bus;
using Domain.Common.Interfaces;
using Domain.Factories;

namespace Domain.Services.Aggregates;

public class BusServices
{
    private readonly IDatetimeProvider _datetimeProvider;

    public BusServices(IDatetimeProvider datetimeProvider)
    {
        _datetimeProvider = datetimeProvider;
    }

    public Bus CreateBus(string id, string name, string tripId, int currentStopIndex)
    {
        return BusFactory.Create(id, name, tripId, currentStopIndex, _datetimeProvider);
    }
}