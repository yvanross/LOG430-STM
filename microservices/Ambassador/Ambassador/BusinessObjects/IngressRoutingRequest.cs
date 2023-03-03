using Ambassador.Properties;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Ambassador.BusinessObjects;

internal class IngressRoutingRequest
{
    public required string TargetService { get; set; }

    private protected static RestClient IngressClient { get; } = new(EnvironmentVariables.IngressAddress);

    internal async Task<RoutingData> Execute()
    {
        try
        {
            var request = new RestRequest(Resources.RouteByServiceType_Endpoint);

            request.AddQueryParameter("serviceType", TargetService);

            var response = await IngressClient.ExecuteGetAsync<RoutingData>(request);

            response.ThrowIfError();

            return response.Data!;
        }
        catch (Exception e)
        {
            EnvironmentVariables.Logger?.LogError("Data received from Ingress was problematic");

            throw;
        }
    }

}