using System.Dynamic;
using System.Web;
using Application.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace RouteTimeProvider.RestClients;

public class TomTomClient : IRouteTimeProvider
{
    private const string TomTomUrl = "https://api.tomtom.com";

    private readonly RestClient _restClient = new(TomTomUrl);

    private readonly string _apiKey = Environment.GetEnvironmentVariable("API_KEY")!;

    public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        var request = new RestRequest($"routing/1/calculateRoute/{HttpUtility.UrlEncode($"{startingCoordinates}:{destinationCoordinates}")}/json");

        request.AddQueryParameter("key", _apiKey);

        var result = await _restClient.ExecuteGetAsync(request);

        result.ThrowIfError();

        // avoiding to create a class for deserialization
        dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(result.Content);

        return (int)(data?.routes[0].summary.travelTimeInSeconds ?? default)!;
    }
}