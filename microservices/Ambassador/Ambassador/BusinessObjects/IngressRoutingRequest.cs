using Ambassador.Properties;
using RestSharp;

namespace Ambassador.BusinessObjects;

internal class IngressRoutingRequest
{
    public required string TargetService { get; set; }

    private protected static RestClient IngressClient { get; } = new(EnvironmentVariables.IngressAddress);

    internal async Task<RoutingData> Execute()
    {
        var request = new RestRequest(Resources.RouteByServiceType_Endpoint);

        request.AddQueryParameter("serviceType", TargetService);

        var response = await IngressClient.ExecuteGetAsync<RoutingData>(request);

        response.ThrowIfError();

        return response.Data!;
    }

}