﻿using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Ambassador;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using Docker.DotNet.Models;
using Entities;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private readonly IPodReadModel _readModelModel;

    public RoutingUC(IPodReadModel readModelModel)
    {
        _readModelModel = readModelModel;
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string sourceId, string type, LoadBalancingMode mode)
    {
        var possibleTargets = HandlePodLocalRouting();

        if (possibleTargets is null)
        {
            var podTypes = _readModelModel.GetAllPodTypes().ToDictionary(k => k.Type);
            var podInstances = _readModelModel.GetAllPods();

            possibleTargets = GetPossibleCommunicationEntryPoints(podTypes, podInstances);
        }

        possibleTargets = LoadBalancing(possibleTargets, mode);

        foreach (var target in possibleTargets!)
        {
            yield return new RoutingData()
            {
                Address = target.HttpRoute
            };
        }

        List<IServiceInstance>? GetPossibleCommunicationEntryPoints(IDictionary<string, IPodType> podTypes, ImmutableList<IPodInstance> podInstances)
        {
            return podInstances.SelectMany(pod =>
            {
                if (podTypes[pod.Type].Gateway is { } gateway)
                    return new[] { pod.ServiceInstances.FirstOrDefault(p => p.Type.Equals(gateway.Type)) };

                return pod.ServiceInstances.ToArray();
            }).DistinctBy(pod => pod is not null).ToList()!;
        }

        List<IServiceInstance>? HandlePodLocalRouting()
        {
            var service = _readModelModel.GetServiceById(sourceId);

            if (string.IsNullOrEmpty(service?.PodId) is false && _readModelModel.GetPodById(service.PodId) is { } pod)
            {
                var target = pod.ServiceInstances.Where(serviceInstance => serviceInstance.Type.Equals(type));

                return target.ToList();
            }

            return null;
        }
    }

    public List<IServiceInstance> LoadBalancing(List<IServiceInstance>? services, LoadBalancingMode mode)
    {
        services = services.Where(t => t.ServiceStatus is ReadyState).ToList();

        return mode switch
        {
            LoadBalancingMode.RoundRobin => new () { services[Random.Shared.Next(0, services.Count - 1)] },
            LoadBalancingMode.Broadcast => services,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}