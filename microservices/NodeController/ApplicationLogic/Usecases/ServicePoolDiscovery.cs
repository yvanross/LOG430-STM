using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.Extensions;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class ServicePoolDiscovery
    {
        public static ImmutableHashSet<string> BannedIds = ImmutableHashSet<string>.Empty;

        private readonly IPodWriteService _podWriteService;
        private readonly IPodReadService _podReadService;
        private readonly IEnvironmentClient _environmentClient;
        private readonly ILogger _logger;
        private readonly IHostInfo _hostInfo;

        public ServicePoolDiscovery(IPodWriteService podWriteService, IPodReadService podReadService, IEnvironmentClient environmentClient, ILogger<ServicePoolDiscovery> logger, IHostInfo hostInfo)
        {
            _podWriteService = podWriteService;
            _podReadService = podReadService;
            _environmentClient = environmentClient;
            _logger = logger;
            _hostInfo = hostInfo;
        }

        public async Task DiscoverServices()
        {
            var unregisteredServices = await GetUnregisteredServices();

            var containerInfos = await Task.WhenAll(unregisteredServices.ConvertAll(_environmentClient.GetContainerInfo));

            //sorting by the number of links (sidecars), so that the services with the most links are registered first
            foreach (var container in containerInfos.OrderByDescending(container => GetPodLinks(container.CuratedInfo).Count()))
            {
                try
                {
                    string podId;

                    if (ServiceAlreadyKnown(container, out var registeredPodInstance))
                    {
                        podId = registeredPodInstance!.Id;
                    }
                    else
                    {
                        if (_podReadService.GetServiceType(GetArtifactName(container.CuratedInfo, container.RawConfig)) is not { } serviceType)
                            serviceType = CreateServiceType(container);

                        if (_podReadService.GetPodType(GetPodTypeName(container.CuratedInfo, container.RawConfig)) is not { } podType)
                            _ = CreatePodType(container, serviceType);

                        var podInstance = GetPodWhereThisInstanceIsNeededToCompletePodInstance(serviceType);

                        podId = podInstance?.Id ?? Guid.NewGuid().ToString();
                    }
                    
                    var serviceInstance = CreateService((container.CuratedInfo, container.RawConfig), podId);

                    RegisterOrUpdatePodInstance((container.CuratedInfo, container.RawConfig), serviceInstance, podId);
                }
                catch (Exception e)
                {
                    //ignore because we don't control the assigned Ids, they are set in the docker compose
                    _logger.LogCritical("Invalid Service configuration found in container pool");
                    _logger.LogTrace(e.ToString());

                    ImmutableInterlocked.Update(ref BannedIds, (set) => set.Add(container.CuratedInfo.Id));
                }
            }

            bool ServiceAlreadyKnown((ContainerInfo CuratedInfo, IContainerConfig RawConfig) valueTuple, out IPodInstance? podInstance)
            {
                var serviceId = valueTuple.RawConfig.Config.Config.Env.SingleOrDefault(env => env.StartsWith("ID="))[3..];

                if (serviceId is not null)
                {
                    var registeredService = _podReadService.GetServiceById(serviceId);

                    if (registeredService is not null)
                    {
                        var registeredPodInstance = _podReadService.GetPodOfService(registeredService);

                        if (registeredPodInstance is not null)
                        {
                            podInstance = registeredPodInstance;

                            return true;

                        }
                    }
                }

                podInstance = null;

                return false;
            }

            async Task<List<string>> GetUnregisteredServices()
            {
                var runningServicesIds = await _environmentClient.GetRunningServices();

                var registeredServices = _podReadService.GetAllServices()
                    .Where(s => s.ContainerInfo is not null && s.ServiceStatus is not LaunchedState).DistinctBy(s => s.Id).ToDictionary(s => s.ContainerInfo!.Id);

                var filterServices = runningServicesIds?.Where(runningService =>
                    BannedIds.Contains(runningService) is false &&
                    registeredServices.ContainsKey(runningService) is false &&
                    runningService.Equals(_hostInfo.GetContainerId()) is false).ToList();

                return filterServices ?? new List<string>();
            }

            IServiceType CreateServiceType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container)
            {
                var serviceType = new ServiceType()
                {
                    ContainerConfig = container.RawConfig,
                    Type = GetArtifactName(container.CuratedInfo, container.RawConfig),
                    ArtifactType = GetArtifactCategory(container.CuratedInfo),
                    DnsAccessibilityModifier = GetPodLinks(container.CuratedInfo).Any() ? Enum.GetName(AccessibilityModifierEnum.Public)! : GetDnsAccessibilityModifier(container.CuratedInfo),
                };

                _podWriteService.AddOrUpdateServiceType(serviceType);

                return serviceType;
            }

            IPodType CreatePodType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container, IServiceType serviceType)
            {
                var podTypeName = GetPodTypeName(container.CuratedInfo, container.RawConfig);

                var newPodType = new PodType(_podReadService)
                {
                    Type = podTypeName,
                };

                newPodType.SetPodLeader(serviceType.Type);

                if (GetNumberOfInstancesAsNumber(container.CuratedInfo) is { } number and > 0)
                    newPodType.SetNumberOfPod(number);

                else if (GetNumberOfInstancesAsHostNames(container.CuratedInfo) is { } hostNames)
                    newPodType.AddRangeHostnames(hostNames);

                var serviceTypes = GetPodLinks(container.CuratedInfo).ToList();

                serviceTypes.Add(serviceType.Type);

                newPodType.AddRangeServiceTypes(serviceTypes);

                _podWriteService.AddOrUpdatePodType(newPodType);

                return newPodType;
            }

            ServiceInstance CreateService((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container, string newPodId)
            {
                if (container.CuratedInfo.PortsInfo.RoutingPortNumber.Equals(default))
                    throw new Exception("Port not found, adding to banned id");

                var newService = new ServiceInstance
                {
                    Id = container.RawConfig.Config.Config.Env.First(e => e.ToString().StartsWith("ID="))[3..],
                    ContainerInfo = container.CuratedInfo,
                    Address = _hostInfo.GetAddress(),
                    Type = GetArtifactName(container.CuratedInfo, container.RawConfig),
                    PodId = GetPodId(container.CuratedInfo) ?? newPodId,
                    ServiceStatus = new ReadyState()
                };

                return newService;
            }

            void RegisterOrUpdatePodInstance((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container, IServiceInstance newService, string podId)
            {
                var pod = _podReadService.GetPodById(podId) ?? new PodInstance(_podReadService)
                {
                    ServiceInstances = ImmutableList<IServiceInstance>.Empty,
                    Type = GetPodTypeName(container.CuratedInfo, container.RawConfig),
                    Id = podId,
                    ServiceStatus = new LaunchedState()
                };

                pod.ReplaceServiceInstance(newService);

                _podWriteService.AddOrUpdatePod(pod);
            }

            string GetPodTypeName(ContainerInfo curatedInfo, IContainerConfig rawConfig)
            {
                return GetPodName(curatedInfo, rawConfig);
            }

            IPodInstance? GetPodWhereThisInstanceIsNeededToCompletePodInstance(IServiceType serviceType)
            {
                var podTypesUsingThisService = _podReadService.GetAllPodTypes()
                    .Where(pt => pt.ServiceTypes.Exists(st => st.Type.EqualsIgnoreCase(serviceType.Type))).ToList();

                var podWhereThisServiceIsNeeded = podTypesUsingThisService
                    .SelectMany(pt => _podReadService.GetPodInstances(pt.Type)
                        .Where(podInstance => podInstance.ServiceStatus is LaunchedState && 
                                              podInstance.ServiceInstances.All(serviceInstance => serviceInstance.Type.EqualsIgnoreCase(serviceType.Type) is false)))
                    .FirstOrDefault();

                return podWhereThisServiceIsNeeded;
            }
        }

        private static string? GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            labels.TryGetValue(serviceLabels, out var label);

            return label;
        }

        private static string GetArtifactName(ContainerInfo infos, IContainerConfig rawConfig)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_NAME, infos.Labels);

            return string.IsNullOrEmpty(value) ? GetArtifactNameFromEnvironmentId() : value;

            string GetArtifactNameFromEnvironmentId()
            {
                return rawConfig.Config.Config.Env
                    .FirstOrDefault(e => e.ToString().StartsWith("ID="))?
                    .Split("=").LastOrDefault()?
                    .Split(".").LastOrDefault() ?? throw new Exception("ID environment variable not defined in compose");
            }
        }

        private static string GetArtifactCategory(ContainerInfo infos)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_CATEGORY, infos.Labels);

            return string.IsNullOrEmpty(value) ? throw new Exception("Artifact category not defined in compose") : value;
        }

        private static int GetNumberOfInstancesAsNumber(ContainerInfo infos)
        {
            uint.TryParse(GetLabelValue(ServiceLabelsEnum.REPLICAS, infos.Labels), out var nbInstances);
            
            return Convert.ToInt32(nbInstances);
        }

        private static IEnumerable<string> GetNumberOfInstancesAsHostNames(ContainerInfo infos)
        {
            return GetLabelValue(ServiceLabelsEnum.REPLICAS, infos.Labels)?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)?
                .ToArray() 
                   ?? Enumerable.Empty<string>();
        }

    private static string GetPodName(ContainerInfo infos, IContainerConfig rawConfig)
        {
            var value = GetLabelValue(ServiceLabelsEnum.POD_NAME, infos.Labels);

            return string.IsNullOrWhiteSpace(value) ? GetPodNameFromEnvironmentId() : value;

            string GetPodNameFromEnvironmentId()
            {
                return rawConfig.Config.Config.Env
                    .FirstOrDefault(e => e.ToString().StartsWith("ID="))?
                    .Split("=").LastOrDefault()?
                    .Split(".").FirstOrDefault() ?? throw new Exception("ID environment variable not defined in compose");
            }
        }

        private static string? GetPodId(ContainerInfo infos)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_ID, infos.Labels);
        }

        private static string GetDnsAccessibilityModifier(ContainerInfo infos)
        {
            var value = GetLabelValue(ServiceLabelsEnum.DNS, infos.Labels);

            return string.IsNullOrEmpty(value) ? nameof(AccessibilityModifierEnum.Private) : value;
        }

        private static IEnumerable<string> GetPodLinks(ContainerInfo infos)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_LINKS, infos.Labels)?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)?
                .ToArray() 
                   ?? Enumerable.Empty<string>();
        }
    }
}
