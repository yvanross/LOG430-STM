using Application.BusinessObjects;
using Application.DTO;
using Application.Interfaces;

namespace Configuration.Stubs;

public class StmClientStub : IBusInfoProvider
{
    public Task<IEnumerable<RideDto>> GetBestBus(string startingCoordinates, string destinationCoordinates)
    {
        return Task.FromResult(new RideDto[]{ new RideDto("0000", "0001", "0002") }.AsEnumerable())!;
    }

    public Task BeginTracking(RideDto bus)
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