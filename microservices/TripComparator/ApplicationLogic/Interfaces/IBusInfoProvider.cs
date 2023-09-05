namespace Entities.DomainInterfaces;

public interface IBusInfoProvider
{
    Task<IEnumerable<IStmBus?>> GetBestBus(string startingCoordinates, string destinationCoordinates);
    
    Task BeginTracking (IStmBus? bus);
    
    Task<IBusTracking?> GetTrackingUpdate(string busId);
}