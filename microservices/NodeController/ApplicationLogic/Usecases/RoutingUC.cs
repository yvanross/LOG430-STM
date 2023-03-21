using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Docker.DotNet.Models;
using Entities;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private readonly IRepositoryRead _readModel;

    private readonly ResourceManagementService _resourceManagementService;

    private const string TomtomUrl = "https://api.tomtom.com";

    public RoutingUC(IRepositoryRead readModel, IRepositoryWrite repositoryWrite, IEnvironmentClient environment)
    {
        _readModel = readModel;
        _resourceManagementService = new ResourceManagementService(environment, readModel, repositoryWrite);
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string sourceId, string type, LoadBalancingMode mode)
    {
        var service = _readModel.GetServiceById(sourceId);

        var possibleTargets = HandlePodLocalRouting();

        if (possibleTargets is null)
        {
            var podTypes = _readModel.GetAllPodTypes().ToDictionary(k => k.Type);
            var podInstances = _readModel.GetAllPods();

            possibleTargets = GetPossibleCommunicationEntryPoints(podTypes, podInstances);
        }

        foreach (var target in possibleTargets!)
        {
            yield return new RoutingData()
            {
                Address = target.IsHttp ? target.HttpRoute : target.HttpsRoute
            };
        }

        IEnumerable<IServiceInstance>? GetPossibleCommunicationEntryPoints(IDictionary<string, IPodType> podTypes, ImmutableList<IPodInstance> podInstances)
        {
            return podInstances.SelectMany(pod =>
            {
                if (podTypes[pod.Type].Gateway is { } gateway)
                    return new[] { pod.ServiceInstances.FirstOrDefault(p => p.Type.Equals(gateway.Type)) };

                return pod.ServiceInstances.ToArray();
            }).DistinctBy(pod => pod is not null).ToList()!;
        }

        IEnumerable<IServiceInstance>? HandlePodLocalRouting()
        {
            if (string.IsNullOrEmpty(service?.PodId) is false && _readModel.GetPodById(service.PodId) is { } pod)
            {
                var target = pod.ServiceInstances.Where(serviceInstance => serviceInstance.Type.Equals(type));

                return target;
            }

            return null;
        }
    }
}