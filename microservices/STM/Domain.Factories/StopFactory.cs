using Domain.Aggregates.Stop;

namespace Domain.Factories;

internal static class StopFactory
{
    internal static Stop Create(string id, double latitude, double longitude)
    {
        return new Stop(id, latitude, longitude);
    }
}