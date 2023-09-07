namespace ServiceMeshHelper.BusinessObjects;

/// <summary>
/// Represents useful information to route a request to a service instance
/// </summary>
public class RoutingData
{
    /// <summary>
    /// The complete URL of the service instance
    /// </summary>
    public required string Address { get; set; }


    /// <summary>
    /// The host name of the service instance
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// The port of the service instance
    /// </summary>
    public required string Port { get; set; }
}