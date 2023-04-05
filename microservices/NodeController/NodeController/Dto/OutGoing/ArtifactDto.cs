using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace NodeController.Dto.OutGoing;

public class ArtifactDto
{
    public string Id { get; init; }

    public string ImageName { get; init; }

    public long NanoCpus { get; init; }

    public long Memory { get; init; }

    public string Type { get; init; }

    public string ServiceStatus { get; init; }

    public string PodId { get; init; }

    public string ArtifactType { get; init; }

    public static ArtifactDto? TryConvertToDto(IServiceInstance serviceInstance, IServiceType? serviceType)
    {
        if (serviceInstance.ContainerInfo is null) return null;

        return new ArtifactDto()
        {
            Id = serviceInstance.Id,
            ImageName = serviceInstance.ContainerInfo.ImageName,
            Memory = serviceInstance.ContainerInfo.Memory,
            NanoCpus = serviceInstance.ContainerInfo.NanoCpus,
            PodId = serviceInstance.PodId,
            ServiceStatus = serviceInstance.ServiceStatus.GetStateName(),
            Type = serviceInstance.Type,
            ArtifactType = serviceType?.ArtifactType ?? "Unknown",
        };
    }
}