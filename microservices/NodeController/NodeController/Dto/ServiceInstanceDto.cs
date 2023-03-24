using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.Live;

public class ServiceInstanceDto
{
    public required string Id { get; init; }

    public required string ImageName { get; init; }

    public required string Status { get; init; }

    public required long NanoCpus { get; init; }

    public required long Memory { get; init; }

    public required string Type { get; init; }

    public required IServiceState ServiceStatus { get; init; }

    public required string PodId { get; init; }
    
    public required string ArtifactType { get; init; }

    public static ServiceInstanceDto? TryConvertToDto(IServiceInstance serviceInstance, IServiceType? serviceType)
    {
        if (serviceInstance.ContainerInfo is null) return null;

        return new ServiceInstanceDto()
        {
            Id = serviceInstance.Id,
            ImageName = serviceInstance.ContainerInfo.ImageName,
            Status = serviceInstance.ContainerInfo.Status,
            Memory = serviceInstance.ContainerInfo.Memory,
            NanoCpus = serviceInstance.ContainerInfo.NanoCpus,
            PodId = serviceInstance.PodId,
            ServiceStatus = serviceInstance.ServiceStatus,
            Type = serviceInstance.Type,
            ArtifactType = serviceType?.ArtifactType ?? "Unknown"
        };
    }
}