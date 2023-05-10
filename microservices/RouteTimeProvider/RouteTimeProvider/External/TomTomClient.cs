using System.Dynamic;
using System.Web;
using Entities.DomainInterfaces;
using Newtonsoft.Json;
using RestSharp;

namespace RouteTimeProvider.External;

public class TomTomClient : IRouteTimeProvider
{
    private RestClient _restClient = new ("https://api.tomtom.com");

    private string _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;

    public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        var request = new RestRequest($"routing/1/calculateRoute/{HttpUtility.UrlEncode($"{startingCoordinates}:{destinationCoordinates}")}/json");

        request.AddQueryParameter("key", _apiKey);

        var result = await _restClient.ExecuteGetAsync(request);

        dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(result.Content);

        return (int)(data?.routes[0].summary.travelTimeInSeconds ?? default)!;
    }
}