using STM.ExternalServiceProvider.Proto;

namespace Application.ReadServices.ServiceInterfaces;

public interface IStmClient
{
    public IEnumerable<VehiclePosition> RequestFeedPositions();

    public Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates();
}