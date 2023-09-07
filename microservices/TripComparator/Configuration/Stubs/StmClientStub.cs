using Application.BusinessObjects;
using Application.DTO;
using Application.Interfaces;

namespace Configuration.Stubs;

public class StmClientStub : IBusInfoProvider
{
    public Task<IEnumerable<Ride>> GetBestBus(string startingCoordinates, string destinationCoordinates)
    {
        return Task.FromResult(new Ride[]{ new Ride("0000", "0001", "0002") }.AsEnumerable())!;
    }

    public Task BeginTracking(Ride bus)
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