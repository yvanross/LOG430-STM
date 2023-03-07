using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Interfaces;

public interface IStmClient
{
    public IEnumerable<VehiclePosition> RequestFeedPositions();

    public Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates();
}