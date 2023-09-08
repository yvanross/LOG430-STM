using Application.DTO;

namespace Application.Interfaces;

public interface IBusInfoProvider
{
    Task<RideDto> GetBestBus(string startingCoordinates, string destinationCoordinates);
    
    Task BeginTracking (RideDto bus);
    
    Task<IBusTracking?> GetTrackingUpdate();
}