namespace Entities.DomainInterfaces;

public interface IRouteTimeProvider
{
    Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates);
}