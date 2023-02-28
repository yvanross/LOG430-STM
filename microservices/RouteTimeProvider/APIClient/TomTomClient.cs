using Entities.DomainInterfaces;
using RestSharp;

namespace APIClient;

public class TomTomClient : IRouteTimeProvider
{
    private RestClient _client = new ("https://api.tomtom.com/");

    public TomTomClient()
    {
        
    }

    public double GetTravelTimeInSeconds(double latitude, double longitude)
    {
        
        var request = new RestRequest("routing");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", "bearer");
        request.AddJsonBody(new { username = "exampleuser", email = "user@example.com" });
        request
        var response = client.Execute(request);
        throw new NotImplementedException();
        "https://developer.tomtom.com/routing-api/documentation/routing/calculate-route";
    }
}