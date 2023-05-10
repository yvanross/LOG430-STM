using System.Collections.Immutable;
using System.Text.RegularExpressions;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Usecases;

public class Routing
{
    private readonly IPodReadService _podReadService;

    public Routing(IPodReadService podReadService)
    {
        _podReadService = podReadService;
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string sourceServiceId, string destinationServiceType, LoadBalancingMode mode)
    {
        var possibleTargets = GetPossibleTargets(destinationServiceType).ToList();

        possibleTargets = LoadBalancing(possibleTargets, mode);

        foreach (var target in possibleTargets!)
        {
            yield return new RoutingData()
            {
                Address = target.HttpRoute
            };
        }

        IEnumerable<IServiceInstance> GetPossibleTargets(string type)
        {
            var (destinationType, destinationNamespace) = GetDestinationNamespaceAndType(type);

            if (sourceServiceId.Equals("$*$") is false)
            {
                var sourcePod = GetSourcePod(sourceServiceId);

                if (sourcePod is null) throw new ArgumentNullException(nameof(sourcePod));

                var checkInLocalPod = CheckInLocalPod(sourcePod, destinationNamespace, destinationType);

                if (checkInLocalPod.Any()) return checkInLocalPod;
            }
            
            var destinationPodTypes = GetDestinationPodType(destinationNamespace, destinationType);

            var destinationPodInstances = destinationPodTypes.SelectMany(podType => _podReadService.GetPodInstances(podType.Type));

            var destinationServices = destinationPodInstances.SelectMany(podInstance => podInstance.ServiceInstances.Where(serviceInstance => serviceInstance.Type.Equals(destinationType)));
            
            return destinationServices;
        }

        (string destinationType, string destinationNamespace) GetDestinationNamespaceAndType(string type)
        {
            var namespaceAndType = type.Split('.', StringSplitOptions.TrimEntries);

            if (namespaceAndType.Any() is false || namespaceAndType.Length > 2)
                throw new InvalidDataException("Target not well formatted, accepted formats: podName.destinationServiceType or destinationServiceType");

            var destinationType = namespaceAndType.Last();

            var destinationNamespace = namespaceAndType.Length < 2 ? string.Empty : namespaceAndType[0];

            return (destinationType, destinationNamespace);
        }

        IPodInstance? GetSourcePod(string sourceId)
        {
            var podId = _podReadService.GetServiceById(sourceId)?.PodId;

            return podId is null ? null : _podReadService.GetPodById(podId);
        }

        IEnumerable<IServiceInstance> CheckInLocalPod(IPodInstance sourcePod, string destinationNamespace, string destinationType)
        {
            if (string.IsNullOrWhiteSpace(destinationNamespace) is false &&
                sourcePod.Type.Equals(destinationNamespace, StringComparison.InvariantCultureIgnoreCase) is false) return Enumerable.Empty<IServiceInstance>();

            return sourcePod.ServiceInstances.Where(si => si.Type.Equals(destinationType, StringComparison.InvariantCultureIgnoreCase));
        }

        IEnumerable<IPodType> GetDestinationPodType(string destinationNamespace, string destinationType)
        {
            return
                from podType in _podReadService.GetAllPodTypes()
                let computedDestinationNamespace = string.IsNullOrWhiteSpace(destinationNamespace)
                    ? podType.Type
                    : destinationNamespace
                where
                    EqualsIgnoreCase(podType.Type, computedDestinationNamespace) &&
                    podType.ServiceTypes.Any(serviceType =>
                        EqualsIgnoreCase(serviceType.Type, destinationType) &&
                        (EqualsIgnoreCase(serviceType.DnsAccessibilityModifier, Enum.GetName(AccessibilityModifierEnum.Public)) ||
                            (EqualsIgnoreCase(serviceType.DnsAccessibilityModifier, Enum.GetName(AccessibilityModifierEnum.Private)) &&
                            EqualsIgnoreCase(computedDestinationNamespace, serviceType.Type))))
                select podType;

            bool EqualsIgnoreCase (string a, string b) => a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public List<IServiceInstance> LoadBalancing(List<IServiceInstance> services, LoadBalancingMode mode)
    {
        services = services.Where(t => t.ServiceStatus is ReadyState).ToList();

        if(services.Any() is false) return services;

        return mode switch
        {
            LoadBalancingMode.RoundRobin => new () { services[Random.Shared.Next(0, services.Count - 1)] },
            LoadBalancingMode.Broadcast => services,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}