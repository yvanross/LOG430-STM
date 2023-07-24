using Microsoft.Extensions.Options;

namespace ServiceMeshHelper.Services;

public class ServiceMeshOptions
{
    public string ServicesAddress { get; set; } = null!;

    public string NodeControllerPort { get; set; } = null!;
}