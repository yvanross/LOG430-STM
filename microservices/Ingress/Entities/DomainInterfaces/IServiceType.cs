using Ingress.Interfaces;

namespace Entities.DomainInterfaces;

public interface IServiceType
{
    string Type { get; set; }

    bool AutoScaleInstances { get; set; }

    int MinimumNumberOfInstances { get; set; }

    IContainerConfigName ContainerConfig { get; set; }
}