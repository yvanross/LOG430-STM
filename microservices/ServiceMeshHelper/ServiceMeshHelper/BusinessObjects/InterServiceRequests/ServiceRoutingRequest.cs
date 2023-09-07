namespace ServiceMeshHelper.BusinessObjects.InterServiceRequests;

/// <summary>
/// Represents information needed by the nuget to route a request to a service instance
/// </summary>
public class ServiceRoutingRequest
{
    /// <summary>
    /// The name of the service to route to (e.g. "RouteTimeProvider"), it is the Id set in the docker compose file
    /// </summary>
    public required string TargetService { get; set; }

    /// <summary>
    /// The endpoint of the service to route to (e.g. "RouteTimeProvider/RouteTime")
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// The query parameters to be passed to the service instance
    /// </summary>
    public List<NameValue> Params { get; set; } = new();

    /// <summary>
    /// The load balancing mode to use, defaults to RoundRobin
    /// </summary>
    public LoadBalancingMode Mode { get; set; } = LoadBalancingMode.RoundRobin;

    private protected ServiceRoutingRequest() { }
}