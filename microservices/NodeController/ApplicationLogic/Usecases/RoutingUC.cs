using System.Collections.Immutable;
using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private readonly IRepositoryRead _repositoryRead;

    private readonly ResourceManagementService _resourceManagementService;

    private const string TomtomUrl = "https://api.tomtom.com";

    public RoutingUC(IRepositoryRead repositoryRead, IRepositoryWrite repositoryWrite, IEnvironmentClient environment)
    {
        _repositoryRead = repositoryRead;
        _resourceManagementService = new ResourceManagementService(environment, repositoryRead, repositoryWrite);
    }

    //todo add local communication in pod
    public IEnumerable<RoutingData> RouteByDestinationType(string type, LoadBalancingMode mode)
    {
        if (type.Equals(ServiceTypes.Tomtom.ToString())){ yield return new RoutingData() { Address = TomtomUrl }; yield break; }

        var pods = _repositoryRead.GetPodInstances(type);

        var podTypes = _repositoryRead.GetAllPodTypes().ToDictionary(k => k.Type);

        var entryServices = pods.SelectMany(pod =>
        {
            if (podTypes[pod.Type].Sidecar is { } entryPoint)
                return new[] { pod.ServiceInstances.FirstOrDefault(p => p.Type.Equals(entryPoint.Type)) };

            return pod.ServiceInstances.ToArray();
        }).DistinctBy(pod => pod is not null).ToImmutableList();

        if (pods is null)
            throw new Exception("service was not found");

        var targets = _resourceManagementService.LoadBalancing(entryServices!, mode);

        foreach (var target in targets)
        {
            yield return new RoutingData()
            {
                Address = target.IsHttp ? target.HttpRoute : target.HttpsRoute
            };
        }
    }
}