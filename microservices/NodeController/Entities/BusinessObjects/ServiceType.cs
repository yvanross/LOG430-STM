using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class ServiceType : IServiceType
{
    public string Type { get; set; }

    public bool AutoScaleInstances { get; set; }

    public int MinimumNumberOfInstances { get; set; }

    public IContainerConfig ContainerConfig { get; set; }
}