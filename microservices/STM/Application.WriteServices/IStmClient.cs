using STM.ExternalServiceProvider.Proto;

namespace Application.CommandServices;

public interface IStmClient
{
    public IEnumerable<VehiclePosition> RequestFeedPositions();

    public Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates();
}