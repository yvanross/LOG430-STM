using Entities.DomainInterfaces;

namespace APIClient;

public class TomTomClient : IRouteTimeProvider
{
    public double GetTravelTimeInSeconds(double latitude, double longitude)
    {
        throw new NotImplementedException();
        "https://developer.tomtom.com/routing-api/documentation/routing/calculate-route";
    }
}