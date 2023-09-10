using System.Threading.Channels;
using RestSharp;
using ServiceMeshHelper.BusinessObjects;
using ServiceMeshHelper.BusinessObjects.InterServiceRequests;
using ServiceMeshHelper.Extensions;
using ServiceMeshHelper.Usecases;

namespace ServiceMeshHelper.Controllers;

/// <summary>
/// Controller exposing Rest actions for inter-service communication.
/// </summary>
public static class RestController
{
    private static readonly Rest RestUc = new ();

    /// <summary>
    /// Create a GET request to the target service by it's name (ID in docker-compose).
    /// Retries 3 times before giving up.
    /// </summary>
    /// <param name="routingRequest"></param>
    /// <returns>RestResponse(s) in an async manner as they become available</returns>
    public static Task<ChannelReader<RestResponse>> Get(GetRoutingRequest routingRequest)
    {
        return Try.WithConsequenceAsync(() => RestUc.Get(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    /// <summary>
    /// Creates a POST request to the target service by it's name (ID in docker-compose).
    /// Retries 3 times before giving up.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routingRequest"></param>
    /// <returns>RestResponse(s) in an async manner as they become available</returns>
    public static Task<ChannelReader<RestResponse>> Post<T>(PostRoutingRequest<T> routingRequest) where T : class
    {
        return Try.WithConsequenceAsync(() => RestUc.Post(routingRequest), retryCount: 3, autoThrow: false)!;
    }

    /// <summary>
    /// Get the address of a service based on it's name (ID in docker-compose) and the load balancing mode.
    /// </summary>
    /// <param name="targetService"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static Task<IEnumerable<RoutingData>> GetAddress(string targetService, LoadBalancingMode mode)
    {
        return RestUc.GetServiceRoutingData(targetService, mode);
    }
}