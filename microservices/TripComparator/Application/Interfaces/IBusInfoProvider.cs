using Application.DTO;

namespace Application.Interfaces;

public interface IBusInfoProvider
{
    Task<IEnumerable<RideDto>> GetBestBus(string startingCoordinates, string destinationCoordinates);
    
    Task BeginTracking (RideDto bus);
    
    Task<IBusTracking?> GetTrackingUpdate(string busId);
}