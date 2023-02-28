using CommunicatorNuget.DomainInterfaces;

namespace CommunicatorNuget.BusinessObjects;

public class RouteTarget : IRouteTarget
{
    public required string Address { get; set; }

    public required string Port { get; set; }
}