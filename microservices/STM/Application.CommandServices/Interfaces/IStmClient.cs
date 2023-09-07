using STM.ExternalServiceProvider.Proto;

namespace Application.CommandServices.Interfaces;

public interface IStmClient
{
    public IEnumerable<VehiclePosition> RequestFeedPositions();

    public Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates();
}