namespace Entities.BusinessObjects.Live;

public record Port(int HostPort, int ContainerPort, string TransportProtocol)
{
    public override string ToString()
    {
        return $"{ContainerPort}/{TransportProtocol}";
    }
}