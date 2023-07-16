using System.Collections.Concurrent;
using Entities.DomainInterfaces.Planned;

namespace Entities.BusinessObjects.Live;

public class ContainerInfo
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string ImageName { get; set; }

    public required string Status { get; set; }

    public required PortsInfo PortsInfo { get; set; }

    public required long NanoCpus { get; set; }
    
    public required long Memory { get; set; }

    public ConcurrentDictionary<ServiceLabelsEnum, string> Labels { get; set; } = new();
}