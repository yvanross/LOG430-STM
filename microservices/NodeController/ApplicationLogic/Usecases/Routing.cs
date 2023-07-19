using System.Collections.Concurrent;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;

namespace ApplicationLogic.Usecases;

public class Routing : IRouting
{
    private readonly IPodReadService _podReadService;
    private readonly IPodWriteService _podWriteService;
    private readonly IHostInfo _hostInfo;

    private readonly ConcurrentDictionary<string, Unresponsive> _unresponsive = new();

    public Routing(IPodReadService podReadService, IPodWriteService podWriteService, IHostInfo hostInfo)
    {
        _podReadService = podReadService;
        _podWriteService = podWriteService;
        _hostInfo = hostInfo;
    }

    public void RegisterUnresponsive(IServiceInstance serviceInstance)
    {
        _unresponsive.AddOrUpdate(serviceInstance.Id, new Unresponsive(serviceInstance, DateTime.UtcNow), (_, _) => new Unresponsive(serviceInstance, DateTime.UtcNow));
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string sourceServiceId, string destinationServiceType, LoadBalancingMode mode)
    {
        var (destinationType, _) = GetDestinationNamespaceAndType(destinationServiceType);

        var serviceType = _podReadService.GetServiceType(destinationType);

        if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));

        return serviceType.ArtifactType.EqualsIgnoreCase(Enum.GetName(ArtifactTypeEnum.Computation)!) ? 
            L7RouteByDestinationType(sourceServiceId, destinationServiceType, mode) : 
            L4RouteByDestinationType(sourceServiceId, destinationServiceType, mode);
    }

    public List<IServiceInstance> LoadBalancing(List<IServiceInstance> services, LoadBalancingMode mode)
    {
        services = services.Where(t => t.ServiceStatus is ReadyState).ToList();

        if (services.Any() is false || mode == LoadBalancingMode.Broadcast) return services;

        var matureServices = services.Where(t => t.ServiceStatus is ReadyState {IsMature: true}).ToList();

        if (matureServices.Any())
            services = matureServices;

        UpdateUnresponsiveServiceList();

        if(services.Where(si => _unresponsive.ContainsKey(si.Id) is false).ToList() is {} responsiveServices && responsiveServices.Any())
            services = responsiveServices;

        return mode switch
        {
            LoadBalancingMode.RoundRobin => new() { services[Random.Shared.Next(0, services.Count - 1)] },
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private void UpdateUnresponsiveServiceList()
    {
        const int unresponsiveThreashold = 10;

        _unresponsive.Where(u => u.Value.checkedIn < DateTime.UtcNow.AddSeconds(-unresponsiveThreashold))
            .ToList()
            .ForEach(u => _unresponsive.TryRemove(u.Key, out _));
    }

    public int NegotiateSocket(IServiceType type)
    {
        var port = _podReadService.TryGetSocketPortForType(type);

        if (port.HasValue && port.Value.Equals(0) is false) return port.Value;

        var takenPorts = _podReadService.GetTakenSocketPorts();

        var availablePorts = _hostInfo.GetTunnelPortRange().Where(tunnelPort => takenPorts.Contains(tunnelPort) is false).ToList();

        if(availablePorts.Any() is false) throw new Exception("No available ports, too many target services types registered");

        port = availablePorts.First();

        _podWriteService.AddTunnel(port.Value, type);

        return port.Value;
    }

    private IEnumerable<RoutingData> L7RouteByDestinationType(string sourceServiceId, string destinationServiceType, LoadBalancingMode mode)
    {
        var possibleTargets = GetPossibleTargets(destinationServiceType, sourceServiceId).ToList();

        possibleTargets = LoadBalancing(possibleTargets, mode);

        foreach (var target in possibleTargets!)
        {
            yield return new RoutingData()
            {
                Address = target.HttpRoute,
                Host = target.Address,
                Port = target.ContainerInfo?.PortsInfo.RoutingPortNumber.ToString() ?? string.Empty,
            };
        }
    }

    private IEnumerable<RoutingData> L4RouteByDestinationType(string sourceServiceId, string destinationServiceType, LoadBalancingMode mode)
    {
        var destinationServiceInstances = _podReadService.GetServiceInstances(destinationServiceType).AsEnumerable();

        destinationServiceInstances = destinationServiceInstances.Where(si => si.Id.EqualsIgnoreCase(sourceServiceId) is false);

        var possibleTargets = LoadBalancing(destinationServiceInstances.ToList(), mode);

        foreach (var target in possibleTargets!)
        {
            yield return new RoutingData()
            {
                Address = target.HttpRoute,
                Host = target.Address,
                Port = target.ContainerInfo?.PortsInfo.RoutingPortNumber.ToString() ?? string.Empty,
            };
        }
    }

    private IEnumerable<IServiceInstance> GetPossibleTargets(string type, string sourceServiceId)
    {
        var (destinationType, destinationNamespace) = GetDestinationNamespaceAndType(type);

        if (sourceServiceId.Equals("$*$") is false)
        {
            var sourcePod = GetSourcePod(sourceServiceId);

            if (sourcePod is null) throw new ArgumentNullException(nameof(sourcePod));

            var checkInLocalPod = CheckInLocalPod(sourcePod, destinationNamespace, destinationType, sourceServiceId);

            if (checkInLocalPod.Any()) return checkInLocalPod;
        }

        var destinationPodTypes = GetDestinationPodType(destinationNamespace, destinationType);

        var destinationPodInstances = destinationPodTypes.SelectMany(podType => _podReadService.GetPodInstances(podType.Type));

        var destinationServices = destinationPodInstances.SelectMany(podInstance => podInstance.ServiceInstances.Where(serviceInstance => serviceInstance.Type.EqualsIgnoreCase(destinationType)));

        destinationServices = destinationServices.Where(si => si.Id.EqualsIgnoreCase(sourceServiceId) is false);

        return destinationServices;
    }

    private (string destinationType, string destinationNamespace) GetDestinationNamespaceAndType(string type)
    {
        var namespaceAndType = type.Split('.', StringSplitOptions.TrimEntries);

        if (namespaceAndType.Any() is false || namespaceAndType.Length > 2)
            throw new InvalidDataException("Target not well formatted, accepted formats: podName.destinationServiceType or destinationServiceType");

        var destinationType = namespaceAndType.Last();

        var destinationNamespace = namespaceAndType.Length < 2 ? string.Empty : namespaceAndType[0];

        return (destinationType, destinationNamespace);
    }

    private IPodInstance? GetSourcePod(string sourceId)
    {
        var podId = _podReadService.GetServiceById(sourceId)?.PodId;

        return podId is null ? null : _podReadService.GetPodById(podId);
    }

    private IEnumerable<IServiceInstance> CheckInLocalPod(IPodInstance sourcePod, string destinationNamespace, string destinationType, string sourceServiceId)
    {
        if (string.IsNullOrWhiteSpace(destinationNamespace) is false &&
            sourcePod.Type.EqualsIgnoreCase(destinationNamespace) is false) return Enumerable.Empty<IServiceInstance>();

        return sourcePod.ServiceInstances.Where(si
            => si.Type.EqualsIgnoreCase(destinationType) && si.Id.EqualsIgnoreCase(sourceServiceId) is false);
    }

    private IEnumerable<IPodType> GetDestinationPodType(string destinationNamespace, string destinationType)
    {
        return
            from podType in _podReadService.GetAllPodTypes()
            let computedDestinationNamespace = string.IsNullOrWhiteSpace(destinationNamespace)
                ? podType.Type
                : destinationNamespace
            where
                podType.Type.EqualsIgnoreCase(computedDestinationNamespace) &&
                podType.ServiceTypes.Any(serviceType =>
                    serviceType.Type.EqualsIgnoreCase(destinationType) &&
                    (serviceType.DnsAccessibilityModifier.EqualsIgnoreCase(Enum.GetName(AccessibilityModifierEnum.Public)) ||
                        (serviceType.DnsAccessibilityModifier.EqualsIgnoreCase(Enum.GetName(AccessibilityModifierEnum.Private)) &&
                        podType.Type.EqualsIgnoreCase(serviceType.Type))))
            select podType;
    }

    private record Unresponsive(IServiceInstance ServiceInstance, DateTime checkedIn);
}