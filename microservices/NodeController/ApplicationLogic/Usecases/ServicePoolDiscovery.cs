using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
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

            foreach (var unregisteredService in unregisteredServices!)
            {
                var newPodId = Guid.NewGuid().ToString();

                var container = await _environmentClient.GetContainerInfo(unregisteredService);

                try
                {
                    var serviceType = CreateServiceType(container);

                    UpdateOrRegisterPodType(container, serviceType);

                    var newService = CreateService(container, newPodId);

                    if (newService is null) continue;

                    RegisterOrUpdatePodInstance(container, newService, newPodId);
                }
                catch
                {
                    // ignore because we don't control the assigned Ids, they are set in the docker compose
                    _logger.LogWarning("Invalid Service configuration found in container pool");

                    ImmutableInterlocked.Update(ref BannedIds, (set) => set.Add(container.CuratedInfo.Id));
                }
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
                var curatedInfoLabels = container.CuratedInfo.Labels;

                var podType = GetPodTypeName(container.CuratedInfo);

                return new ServiceType()
                {
                    ContainerConfig = container.RawConfig,
                    Type = GetArtifactName(curatedInfoLabels),
                    ArtifactType = GetArtifactCategory(curatedInfoLabels),
                    DnsAccessibilityModifier = GetDnsAccessibilityModifier(curatedInfoLabels),
                    PodName = podType
                };
            }

            void UpdateOrRegisterPodType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container, IServiceType newServiceType)
            {
                var podTypeName = GetPodTypeName(container.CuratedInfo);

                var podType = _podReadService.GetPodType(podTypeName);

                if (podType is null || podType.ServiceTypes.Any(serviceType => serviceType.Type.Equals(newServiceType.Type)) is false)
                {
                    _podWriteService.AddOrUpdatePodType(new PodType()
                    {
                        Type = GetPodTypeName(container.CuratedInfo),
                        MinimumNumberOfInstances = GetMinimumNumberOfInstances(container.CuratedInfo.Labels),
                        ServiceTypes = podType?.ServiceTypes.Add(newServiceType) ?? ImmutableList<IServiceType>.Empty.Add(newServiceType),
                    });
                }
            }

            ServiceInstance? CreateService((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, string newPodId)
            {
                if (string.IsNullOrWhiteSpace(service.CuratedInfo.HostPort))
                    throw new Exception("Port not found, adding to banned id");

                var newService = new ServiceInstance
                {
                    Id = service.RawConfig.Config.Config.Env.First(e => e.ToString().StartsWith("ID="))[3..],
                    ContainerInfo = service.CuratedInfo,
                    Address = _hostInfo.GetAddress(),
                    Type = GetArtifactName(service.CuratedInfo.Labels),
                    PodId = GetPodId(service.CuratedInfo.Labels) ?? newPodId,
                    ServiceStatus = new ReadyState()
                };

                return newService;
            }

            void RegisterOrUpdatePodInstance((ContainerInfo CuratedInfo, IContainerConfig RawConfig) container, IServiceInstance newService, string newPodId)
            {
                var podId = GetPodId(container.CuratedInfo.Labels) ?? newPodId;

                var pod = _podReadService.GetPodById(podId) ?? new PodInstance()
                {
                    ServiceInstances = ImmutableList<IServiceInstance>.Empty,
                    Type = GetPodTypeName(container.CuratedInfo),
                    Id = podId
                };

                pod.ServiceInstances = pod.ServiceInstances.RemoveAll(s => s.Id.Equals(newService.Id));
                pod.ServiceInstances = pod.ServiceInstances.Add(newService);

                _podWriteService.AddOrUpdatePod(pod);
            }

            string GetPodTypeName(ContainerInfo curatedInfo)
            {
                return GetPodName(curatedInfo.Labels) ?? GetArtifactName(curatedInfo.Labels);
            }
        }

        private static string? GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            labels.TryGetValue(serviceLabels, out var label);

            return label;
        }

        private static string GetArtifactName(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_NAME, labels);

            return string.IsNullOrEmpty(value) ? throw new Exception("Artifact name not defined in compose") : value;
        }

        private static string GetArtifactCategory(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_CATEGORY, labels);

            return string.IsNullOrEmpty(value) ? nameof(ArtifactTypeEnum.Undefined) : value;
        }

        private static int GetMinimumNumberOfInstances(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            uint.TryParse(GetLabelValue(ServiceLabelsEnum.MINIMUM_NUMBER_OF_INSTANCES, labels), out var nbInstances);
            
            return Convert.ToInt32(nbInstances);
        }

        private static string? GetPodName(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_NAME, labels);
        }

        private static string? GetPodId(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            return GetLabelValue(ServiceLabelsEnum.POD_ID, labels);
        }

        private static string GetDnsAccessibilityModifier(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.DNS, labels);

            return string.IsNullOrEmpty(value) ? nameof(AccessibilityModifierEnum.Private) : value;
        }
    }
}
