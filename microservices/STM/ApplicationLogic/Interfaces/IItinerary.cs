using Entities.Common.Interfaces;
using Entities.Transit.Interfaces;

namespace ApplicationLogic.Use_Cases;

public interface IItinerary
{
    Task<(IBus bus, double eta)[]?> GetFastestBus(IPosition from, IPosition to);
    Task PrefetchAndApplyTripUpdates();
}