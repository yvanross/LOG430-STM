using Domain.Aggregates.Bus;
using Domain.Common.Interfaces;

namespace Domain.Factories;

internal static class BusFactory
{
    internal static Bus Create(string id, string name, string tripId, int currentStopIndex, IDatetimeProvider datetimeProvider)
    {
        var bus = new Bus(id, name, tripId, currentStopIndex, datetimeProvider);

        return bus;
    }
}