using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using TripComparator.DTO;

namespace Configuration.Stubs;

public class StmClientStub : IBusInfoProvider
{
    public Task<IEnumerable<IStmBus?>> GetBestBus(string startingCoordinates, string destinationCoordinates)
    {
        return Task.FromResult(new IStmBus[]{ new StmBusDto() }.AsEnumerable())!;
    }

    public Task BeginTracking(IStmBus? bus)
    {
        return Task.CompletedTask;
    }

    public Task<IBusTracking?> GetTrackingUpdate(string busId)
    {
        return Task.FromResult((IBusTracking) new BusTracking()
        {
            Duration = 1,
            Message = "i'm a tripcomparator stm client stub",
            TrackingCompleted = false
        })!;
    }
}