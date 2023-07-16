using System.Threading.Channels;
using RestSharp;
using ServiceMeshHelper.Bo;
using ServiceMeshHelper.Bo.InterServiceRequests;
using ServiceMeshHelper.Extensions;
using ServiceMeshHelper.Usecases;

namespace ServiceMeshHelper.Controllers;

public static class RestController
{
    private static readonly Rest RestUc = new ();

    public static Task<ChannelReader<RestResponse>> Get(GetRoutingRequest routingRequest)
    {
        return Try.WithConsequenceAsync(() => RestUc.Get(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    public static Task<ChannelReader<RestResponse>> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
    {
        return Try.WithConsequenceAsync(() => RestUc.Post(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    public static Task<IEnumerable<RoutingData>> GetAddress(string targetService, LoadBalancingMode mode)
    {
        return RestUc.GetServiceRoutingData(targetService, mode);
    }
}