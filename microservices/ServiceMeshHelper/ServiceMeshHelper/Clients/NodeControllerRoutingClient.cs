using System.Net;
using RestSharp;
using ServiceMeshHelper.BusinessObjects;
using ServiceMeshHelper.Extensions;
using ServiceMeshHelper.Services;

namespace ServiceMeshHelper.Clients;

internal class NodeControllerRoutingClient
{
    private static RestClient NodeControllerClient { get; } = new(ServiceMeshConfiguration.NodeControllerAddress);

    internal async Task<IEnumerable<RoutingData>> RouteByServiceType(string serviceType, LoadBalancingMode routingRequestMode)
        => await Route(serviceType, routingRequestMode);

    internal async Task<int> NegotiateSocketForServiceType(string serviceType)
        => await NegotiateSocket(serviceType);

    private async Task<IEnumerable<RoutingData>> Route(string serviceType, LoadBalancingMode routingRequestMode)
    {
        const int retryAttempts = 5;

        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"Routing/RouteByServiceType/{serviceType}");

            request.AddQueryParameter("caller", ServiceMeshConfiguration.ServiceId);

            request.AddQueryParameter("mode", routingRequestMode);

            var response = await NodeControllerClient.ExecuteAsync<IEnumerable<RoutingData>>(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Couldn't route to service type, more info may be provided in the NodeController's logs. The response received from the NodeController: {response.Content}");
            }

            response.ThrowIfError();

            return response.Data!;
        },
            (_, i) => Task.Delay(TimeSpan.FromSeconds(i / 5.0)), retryAttempts, quiet: true);
    }

    private async Task<int> NegotiateSocket(string serviceType)
    {
        const int retryAttempts = 10;

        return await Try.WithConsequenceAsync(async () =>
        {
            var request = new RestRequest($"Routing/NegotiateSocket/{serviceType}", Method.Post);

            var response = await NodeControllerClient.ExecuteAsync<int>(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception("No available sockets for service type. Check the NodeController for logs. This can happen if no viable registered service exist for the tcp connection");
            }

            response.ThrowIfError();

            return response.Data!;
        },
            (_, i) => Task.Delay(TimeSpan.FromSeconds(i / 5.0)), retryAttempts, quiet: true);
    }
}