namespace ServiceMeshHelper;

/// <summary>
/// Enum to specify the load balancing mode for a request
/// </summary>
public enum LoadBalancingMode
{
    /// <summary>
    /// Will randomly select a matching service instance
    /// </summary>
    RoundRobin,
    /// <summary>
    /// Will send the request to all matching service instances
    /// </summary>
    Broadcast
}