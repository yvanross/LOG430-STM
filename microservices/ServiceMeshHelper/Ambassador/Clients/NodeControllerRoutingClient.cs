using Polly;
using RestSharp;
using ServiceMeshHelper.BusinessObjects;
using ServiceMeshHelper.Services;

namespace ServiceMeshHelper.Clients;

internal class NodeControllerRoutingClient
{
    private static RestClient NodeControllerClient { get; } = new(ServiceMeshConfiguration.NodeControllerAddress);

    private const int _retryCount = 3;

    private static AsyncPolicy _retry = Policy.Handle<Exception>()
        .WaitAndRetryAsync(_retryCount, attempt => TimeSpan.FromSeconds(Math.Max(attempt / 2, 5)),
            (exception, _, retryCount, _) =>
            {
                if(retryCount.Equals(_retryCount)) 
                    Console.WriteLine(exception);
            });

    internal async Task<IEnumerable<RoutingData>> RouteByServiceType(string serviceType, LoadBalancingMode routingRequestMode)
        => await _retry.ExecuteAsync(() => Route(serviceType, routingRequestMode));

    internal async Task<int> NegotiateSocketForServiceType(string serviceType)
        => await _retry.ExecuteAsync(() => NegotiateSocket(serviceType));

    private async Task<IEnumerable<RoutingData>> Route(string serviceType, LoadBalancingMode routingRequestMode)
    {
        var request = new RestRequest($"Routing/RouteByServiceType/{serviceType}");

        request.AddQueryParameter("caller", ServiceMeshConfiguration.ServiceId);

        request.AddQueryParameter("mode", routingRequestMode);

        var response = await NodeControllerClient.ExecuteAsync<IEnumerable<RoutingData>>(request);

        response.ThrowIfError();

        return response.Data!;
    }

    private async Task<int> NegotiateSocket(string serviceType)
    {
        var request = new RestRequest($"Routing/NegotiateSocket/{serviceType}", Method.Post);

        var response = await NodeControllerClient.ExecuteAsync<int>(request);

        response.ThrowIfError();

        return response.Data!;
    }
}