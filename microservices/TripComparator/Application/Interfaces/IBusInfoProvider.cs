using Application.DTO;

namespace Application.Interfaces;

public interface IBusInfoProvider
{
    Task<IEnumerable<Ride>> GetBestBus(string startingCoordinates, string destinationCoordinates);
    
    Task BeginTracking (Ride bus);
    
    Task<IBusTracking?> GetTrackingUpdate(string busId);
}