namespace CommunicatorNuget.DomainInterfaces;

public interface IRouteTarget
{
    public string Address { get; set; }

    public string Port { get; set; }
}