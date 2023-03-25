﻿using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.BusinessObjects.Planned;

public class ServiceType : IServiceType
{
    public required string Type { get; set; }

    public required IContainerConfig ContainerConfig { get; set; }

    public string ArtifactType { get; set; } = ArtifactTypeEnum.Undefined.ToString();
    
    public bool IsPodSidecar { get; set; }
    
    public string PodName { get; set; }
}