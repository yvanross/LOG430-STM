using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Moq;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Entities.Dao;
using Microsoft.Extensions.Logging;

namespace ApplicationLogicTests;

public static class MockProvider
{
    public static Mock<ILogger> GetLoggerMock()
    {
        var logger = new Mock<ILogger>();

        logger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<object>() ,
            It.IsAny<Exception>(),
            It.IsAny<Func<object, Exception?, string>>()));
       
        return logger;
    }

    public static Mock<IHostInfo> GetHostInfoMock()
    {
        var hostInfo = new Mock<IHostInfo>();

        hostInfo.Setup(x => x.GetAddress()).Returns("http://testAddress");
        hostInfo.Setup(x => x.GetIngressAddress()).Returns("http://testAddress");
        hostInfo.Setup(x => x.GetIngressPort()).Returns("0000");
        hostInfo.Setup(x => x.GetPort()).Returns("0000");
        hostInfo.Setup(x => x.GetMQServiceName()).Returns("mq");
        hostInfo.Setup(x => x.GetTeamName()).Returns("teamTest");

        return hostInfo;
    }

    public static Mock<IIngressClient> GetIngressClientMock()
    {
        var ingressClient = new Mock<IIngressClient>();

        ingressClient.Setup(x => x.GetLogStoreAddressAndPort(It.IsAny<string>())).Returns(Task.FromResult("http://testAddress:8329"));

        ingressClient.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        return ingressClient;
    }

    public static Mock<IBusPositionUpdated> GetSagaMock(int phase = 0, int seconds = 1)
    {
        var saga = new Mock<IBusPositionUpdated>();

        saga.Setup(x => x.Message).Returns("Test message");

        saga.Setup(x => x.Seconds).Returns(seconds);

        return saga;
    }

    public static Mock<IDataStreamService> GetDataStreamReadMock()
    {
        var dataStreamReadModel = new Mock<IDataStreamService>();

        dataStreamReadModel.Setup(x => x.Produce(It.IsAny<ICoordinates>()));

        return dataStreamReadModel;
    }

    public static Mock<ISystemStateWriteService> GetSystemStateWriteMock()
    {
        var SystemStateWriteModel = new Mock<ISystemStateWriteService>();

        SystemStateWriteModel.Setup(x => x.Log(It.IsAny<IExperimentReport>()));

        return SystemStateWriteModel;
    }

    public static Mock<IChaosCodex> GetChaosCodexMock(params KeyValuePair<ArtifactTypeEnum, IChaosConfig>[] preMocked)
    {
        var chaosCodex = new Mock<IChaosCodex>();

        var chaosConfigs = new ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>();
        chaosConfigs.TryAdd(ArtifactTypeEnum.Computation, GetChaosConfigMock().Object);
        chaosConfigs.TryAdd(ArtifactTypeEnum.Database, GetChaosConfigMock().Object);
        chaosConfigs.TryAdd(ArtifactTypeEnum.Connector, GetChaosConfigMock().Object);

        foreach (var chaosConfig in preMocked)
        {
            chaosConfigs.Remove(chaosConfig.Key, out _);
            chaosConfigs.TryAdd(chaosConfig.Key, chaosConfig.Value);
        }

        chaosCodex.Setup(x => x.EndTestAt).Returns(DateTime.UtcNow.AddSeconds(2));
        chaosCodex.Setup(x => x.StartTestAt).Returns(DateTime.UtcNow);
        chaosCodex.Setup(x => x.ChaosConfigs).Returns(chaosConfigs);

        return chaosCodex;
    }

    public static Mock<IChaosConfig> GetChaosConfigMock()
    {
        var chaosConfig = new Mock<IChaosConfig>();

        chaosConfig.Setup(x => x.KillRate).Returns(100);
        chaosConfig.Setup(x => x.MaxNumberOfPods).Returns(10);
        chaosConfig.Setup(x => x.Memory).Returns(1000);
        chaosConfig.Setup(x => x.NanoCpus).Returns(1000);

        return chaosConfig;
    }

    public static Mock<IPodWriteService> GetWriteModelMock()
    {
        var writeModel = new Mock<IPodWriteService>();

        writeModel.Setup(x => x.AddOrUpdatePod(It.IsAny<IPodInstance>()));

        writeModel.Setup(x => x.TryRemovePod(It.IsAny<IPodInstance>()));

        writeModel.Setup(x => x.AddOrUpdatePodType(It.IsAny<IPodType>()));

        return writeModel;
    }

    public static Mock<IPodReadService> GetReadModelMock()
    {
        var podInstance1 = GetPodInstanceMock(1, 1).Object;
        var podInstance2 = GetPodInstanceMock(2, 1).Object;
        var podInstance3 = GetPodInstanceMock(1, 2).Object;

        var allPods = ImmutableList<IPodInstance>.Empty
            .Add(podInstance1)
            .Add(podInstance2)
            .Add(podInstance3);

        var allServices = ImmutableList<IServiceInstance>.Empty
            .AddRange(podInstance1.ServiceInstances)
            .AddRange(podInstance2.ServiceInstances)
            .AddRange(podInstance3.ServiceInstances);

        var podType1 = GetPodTypeMock(1).Object;
        var podType2 = GetPodTypeMock(2).Object;

        var readModel = new Mock<IPodReadService>();

        readModel.Setup(x => x.GetAllPodTypes())
            .Returns(ImmutableList<IPodType>.Empty.Add(podType1).Add(podType2));

        readModel.Setup(x => x.GetAllPods())
            .Returns(ImmutableList<IPodInstance>.Empty
                .Add(podInstance1)
                .Add(podInstance2)
                .Add(podInstance3));

        readModel.Setup(x => x.GetPodById(It.IsAny<string>())).Returns(podInstance1);

        readModel.Setup(x => x.GetPodInstances(It.IsAny<string>())).Returns(allPods);

        readModel.Setup(x => x.GetPodOfService(It.IsAny<IServiceInstance>())).Returns(podInstance1);

        readModel.Setup(x => x.GetPodType(It.IsAny<string>())).Returns(podType1);

        readModel.Setup(x => x.GetAllServiceTypes())
            .Returns(ImmutableList<IServiceType>.Empty
                .Add(GetServiceTypeMock(1).Object)
                .Add(GetServiceTypeMock(2).Object));

        readModel.Setup(x => x.GetAllServices())
            .Returns(allServices);

        readModel.Setup(x => x.GetServiceType(It.IsAny<string>())).Returns(podType1.ServiceTypes.First());

        readModel.Setup(x => x.GetServiceById(It.IsAny<string>())).Returns(podInstance1.ServiceInstances.First());

        readModel.Setup(x => x.GetServiceInstances(It.IsAny<string>()))
            .Returns(allServices.GroupBy(x => x.Type).MaxBy(x => x.Count())!.ToImmutableList);

        return readModel;
    }

    public static Mock<IPodInstance> GetPodInstanceMock(int podInstanceNb, int podTypeNb)
    {
        var podinstance = new Mock<IPodInstance>();

        podinstance.Setup(x => x.Id).Returns($"podInstance{podInstanceNb}");

        podinstance.Setup(x => x.ServiceInstances)
            .Returns(ImmutableList<IServiceInstance>.Empty
                .Add(GetServiceInstanceMock(1 + (podInstanceNb * 10), podInstanceNb, podTypeNb, 1).Object)
                .Add(GetServiceInstanceMock(2 + (podInstanceNb * 10), podInstanceNb, podTypeNb, 2).Object));

        podinstance.Setup(x => x.Type).Returns(GetPodTypeMock(podTypeNb).Object.Type);

        return podinstance;
    }

    public static Mock<IPodType> GetPodTypeMock(int podTypeNb)
    {
        var podType = new Mock<IPodType>();

        podType.Setup(x => x.NumberOfInstances).Returns(2);
        podType.Setup(x => x.ServiceTypes)
            .Returns(ImmutableList<IServiceType>.Empty.Add(GetServiceTypeMock(1).Object)
                .Add(GetServiceTypeMock(2).Object));

        podType.Setup(x => x.Type).Returns($"PodType{podTypeNb}");

        return podType;
    }

    public static Mock<IServiceType> GetServiceTypeMock(int serviceTypeNb)
    {
        var serviceType = new Mock<IServiceType>();

        var containerConfig = new Mock<IContainerConfig>();
        /*
        var response = new ContainerInspectResponse
        {
            Config = new
            {
                Env = new List<string>() { $"ID=ServiceType{serviceTypeNb}" }
            }
        };

        containerConfig.SetupGet(x => x.Config).Returns(response);

        serviceType.Setup(x => x.ArtifactType).Returns(Enum.GetName(ArtifactTypeEnum.Computation)!);
        serviceType.Setup(x => x.ContainerConfig).Returns(containerConfig.Object);
        serviceType.Setup(x => x.DnsAccessibilityModifier).Returns(false);
        serviceType.Setup(x => x.Type).Returns($"ServiceType{serviceTypeNb}");
        */
        return serviceType;
    }

    public static Mock<IServiceInstance> GetServiceInstanceMock(int serviceInstanceNb, int podInstanceNb, int podTypeNb, int serviceTypeNb)
    {
        var serviceInstance = new Mock<IServiceInstance>();

        var containerInfo = CreateContainerInfo(serviceInstanceNb, podInstanceNb, podTypeNb);

        serviceInstance.Setup(x => x.Address).Returns("host.docker.internal");
        serviceInstance.Setup(x => x.ContainerInfo).Returns(containerInfo);
        serviceInstance.Setup(x => x.HttpRoute).Returns($"{serviceInstanceNb}");
        serviceInstance.Setup(x => x.Id).Returns($"{podInstanceNb}10462F39-5E35-43E8-999E-F723CA48306B{serviceInstanceNb}");
        serviceInstance.Setup(x => x.PodId).Returns($"PodId{podInstanceNb}");
        serviceInstance.Setup(x => x.ServiceStatus).Returns(new ReadyState());
        serviceInstance.Setup(x => x.Type).Returns(GetServiceTypeMock(serviceTypeNb).Object.Type);

        return serviceInstance;
    }

    public static Mock<IScheduler> GetSchedulerMock()
    {
        var scheduler = new Mock<IScheduler>();

        scheduler.Setup(x => x.TryAddTask(It.IsAny<string>(), It.IsAny<Func<Task>>()));
        scheduler.Setup(x => x.TryRemoveTask(It.IsAny<string>()));

        return scheduler;
    }

    public static Mock<IEnvironmentClient> GetEnvironmentMock()
    {
        var readModel = GetReadModelMock();

        var serviceInstance1 = readModel.Object.GetAllServices()[0];
        var serviceInstance2 = readModel.Object.GetAllServices()[1];
        var serviceType = GetServiceTypeMock(Convert.ToInt32(serviceInstance1.Type.Last()));

        var environmentClient = new Mock<IEnvironmentClient>();

        environmentClient.Setup(x => x.GarbageCollection()).Returns(Task.CompletedTask);

        environmentClient.Setup(x => x.GetContainerInfo(It.IsAny<string>()))
            .Returns(Task.FromResult((serviceInstance1.ContainerInfo, serviceType.Object.ContainerConfig))!);

        environmentClient.Setup(x => x.GetRunningServices(null))
            .Returns(Task.FromResult(readModel.Object.GetAllServices().ConvertAll(s=>s.ContainerInfo.Id)));

        //environmentClient.Setup(x => x.RemoveContainerInstance(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        environmentClient.Setup(x => x.SetResources(It.IsAny<IPodInstance>(), It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        return environmentClient;
    }

    public static ContainerInfo CreateContainerInfo(int serviceInstanceNb, int podInstanceNb, int podTypeNb)
    {
        var containerId = $"TestId{serviceInstanceNb}";

        var containerInfo = new ContainerInfo()
        {
            Id = containerId,
            ImageName = "TestImage",
            Memory = 100000L,
            NanoCpus = 100000L,
            Name = $"ContainerTest{serviceInstanceNb}",
            PortsInfo = new PortsInfo(),
            Status = "Testing",
            Labels = new ConcurrentDictionary<ServiceLabelsEnum, string>(),
        };

        containerInfo.Labels.TryAdd(ServiceLabelsEnum.ARTIFACT_CATEGORY, "Computation");
        containerInfo.Labels.TryAdd(ServiceLabelsEnum.ARTIFACT_NAME, $"{podTypeNb}");
        containerInfo.Labels.TryAdd(ServiceLabelsEnum.REPLICAS, "2");
        containerInfo.Labels.TryAdd(ServiceLabelsEnum.POD_ID, $"PodId{podInstanceNb}");
        containerInfo.Labels.TryAdd(ServiceLabelsEnum.POD_NAME, $"PodType{podTypeNb}");

        return containerInfo;
    }
}