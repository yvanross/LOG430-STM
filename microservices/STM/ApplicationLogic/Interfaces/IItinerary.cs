using Domain.ValueObjects;

namespace ApplicationLogic.Interfaces;

public interface IItinerary
{
    Task<(IBus bus, double eta)[]?> GetFastestBus(Position from, Position to);

    Task PrefetchAndApplyTripUpdates();
}