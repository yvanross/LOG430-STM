namespace Entities.DomainInterfaces;

public interface IRouteTimeProvider
{
    double GetTravelTimeInSeconds(double latitude, double longitude);
}