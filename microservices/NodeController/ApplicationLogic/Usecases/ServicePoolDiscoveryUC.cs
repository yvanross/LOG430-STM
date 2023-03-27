using System.Collections.Concurrent;
using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.Planned;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace ApplicationLogic.Usecases
{
    public class ServicePoolDiscoveryUC
    {
        private readonly IPodWriteModel _podWriteModel;
        
        private readonly IPodReadModel _podReadModel;

        private readonly IEnvironmentClient _environmentClient;

        private static string NodeAddress => Environment.GetEnvironmentVariable("SERVICES_ADDRESS")!;

        public ServicePoolDiscoveryUC(IPodWriteModel podWriteModel, IPodReadModel podReadModel, IEnvironmentClient environmentClient)
        {
            _podWriteModel = podWriteModel;
            _podReadModel = podReadModel;
            _environmentClient = environmentClient;
        }

        public async Task DiscoverServices()
        {
            var unregisteredServices = await GetUnregisteredServices();

            foreach (var unregisteredService in unregisteredServices!)
            {
                var newPodId = Guid.NewGuid().ToString();

                var service = await _environmentClient.GetContainerInfo(unregisteredService);

                var newService = CreateService(service, newPodId);

                if(newService is null) continue;

                CreateServiceType(service);

                CreateOrUpdatePodInstance(service, newService, newPodId);
            }

            async Task<List<string>> GetUnregisteredServices()
            {
                var runningServicesIds = await _environmentClient.GetRunningServices();

                var registeredServices = _podReadModel.GetAllServices().DistinctBy(s=>s.Id).ToDictionary(s => s.Id);

                var unregisteredServices = runningServicesIds?.Where(runningService => registeredServices.ContainsKey(runningService) is false).ToList();
                
                return unregisteredServices ?? new List<string>();
            }

            ServiceInstance? CreateService((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, string newPodId)
            {
                try
                {
                    var newService = new ServiceInstance()
                    {
                        Id = service.RawConfig.Config.Config.Env.First(e => e.ToString().StartsWith("ID=")),
                        ContainerInfo = service.CuratedInfo,
                        Address = NodeAddress,
                        Type = service.CuratedInfo.Name,
                        PodId = GetPodId(service.CuratedInfo.Labels) ?? newPodId
                    };

                    newService.ServiceStatus = new ReadyState();

                    return newService;
                }
                catch
                {
                    // ignore because we don't control the assigned Ids, they are set in the docker composee
                }
                
                return null;
            }

            void CreateServiceType((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service)
            {
                var curatedInfoLabels = service.CuratedInfo.Labels;

                var podType = GetPodName(curatedInfoLabels) ?? service.CuratedInfo.Name;

                var serviceType = new ServiceType()
                {
                    ContainerConfig = service.RawConfig,
                    Type = service.CuratedInfo.Name,
                    ArtifactType = GetComponentCategory(curatedInfoLabels),
                    IsPodSidecar = GetIsSidecar(curatedInfoLabels),
                    PodName = podType
                };

                UpdateOrCreatePodType(podType, service, serviceType);
            }

            void UpdateOrCreatePodType(string podType, (ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceType serviceType)
            {
                var pod = _podReadModel.GetPodType(podType);

                if (pod is null)
                {
                    _podWriteModel.AddOrUpdatePodType(new PodType()
                    {
                        Type = podType,
                        MinimumNumberOfInstances = GetMinimumNumberOfInstances(service.CuratedInfo.Labels),
                        Gateway = GetIsSidecar(service.CuratedInfo.Labels) ? serviceType : pod?.Gateway ?? default,
                        ServiceTypes = pod?.ServiceTypes.Add(serviceType) ?? ImmutableList<IServiceType>.Empty.Add(serviceType),
                    });
                }
            }

            void CreateOrUpdatePodInstance((ContainerInfo CuratedInfo, IContainerConfig RawConfig) service, IServiceInstance newService, string newPodId)
            {
                var podId = GetPodId(service.CuratedInfo.Labels) ?? newPodId;

                var pod = _podReadModel.GetPodById(podId);

                pod ??= new PodInstance()
                {
                    ServiceInstances = pod?.ServiceInstances?.Add(newService) ??
                                       ImmutableList<IServiceInstance>.Empty.Add(newService),
                    Type = GetPodName(service.CuratedInfo.Labels) ?? service.CuratedInfo.Name,
                    Id = podId
                };

                _podWriteModel.AddOrUpdatePod(pod);
            }
        }

        private static string? GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            labels.TryGetValue(serviceLabels, out var label);

            return label;
        }

        private static string GetComponentCategory(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            var value = GetLabelValue(ServiceLabelsEnum.COMPONENT_CATEGORY, labels);

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

        private static bool GetIsSidecar(ConcurrentDictionary<ServiceLabelsEnum, string> labels)
        {
            bool.TryParse(GetLabelValue(ServiceLabelsEnum.IS_POD_SIDECAR, labels), out var isSidecar);

            return isSidecar;
        }
    }
}
