namespace Entities.BusinessObjects.Live;

public class PortsInfo
{
    /// <summary>
    /// This is the port used be the load balancing and routing service to route traffic to the container.
    /// </summary>
    public int RoutingPortNumber { get; set; }

    public List<Port> Ports { get; set; }
}