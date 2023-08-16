using Domain.Aggregates;
using Domain.ValueObjects;

namespace Domain.Factories;

internal static class StopFactory
{
    internal static Stop Create(string id, Position position)
    {
        return new Stop(id, position);
    }
}