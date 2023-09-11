namespace Application.Interfaces;

public interface IRouteTimeProvider
{
    Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates);
}