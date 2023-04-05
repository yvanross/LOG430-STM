using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.Live;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class MonitorTest
    {
        private MonitorUc monitorUc;

        private Mock<IEnvironmentClient> _envMock;

        private Mock<IPodReadService> _podReadModel;

        [TestInitialize]
        public void Init()
        {
            _envMock = MockProvider.GetEnvironmentMock();
            _podReadModel = MockProvider.GetReadModelMock();

            monitorUc = new MonitorUc(
                _envMock.Object,
                _podReadModel.Object,
                MockProvider.GetWriteModelMock().Object);
        }

        [TestMethod()]
        public async Task ProcessPodStates_PodsNeedToBeRemoved()
        {
            _envMock.Setup(x => x.GetRunningServices(It.IsAny<string[]>())).Returns(Task.FromResult(ImmutableList<string>.Empty)!);

            await monitorUc.RemoveOrReplaceDeadPodsFromModel();

            _envMock.Verify(x=>x.RemoveContainerInstance(
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(6));
        }

        [TestMethod()]
        public async Task ProcessPodStates_NoPodsNeedToBeRemoved()
        {
            await monitorUc.RemoveOrReplaceDeadPodsFromModel();

            _envMock.Verify(x => x.RemoveContainerInstance(
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(0));
        }

        [TestMethod()]
        public async Task GarbageCollection()
        {
            await monitorUc.GarbageCollection();
        }

        [TestMethod()]
        public async Task MatchDemandOnPods_NeedsMorePods()
        {
            _podReadModel.Setup(x => x.GetPodInstances(It.IsAny<string>())).Returns(ImmutableList<IPodInstance>.Empty);

            await monitorUc.MatchInstanceDemandOnPods();

            _envMock.Verify(x => x.IncreaseByOneNumberOfInstances(
                    It.IsAny<IContainerConfig>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(4));
        }

        [TestMethod()]
        public async Task MatchDemandOnPods_NoNeedForMorePods()
        {
            await monitorUc.MatchInstanceDemandOnPods();

            _envMock.Verify(x => x.IncreaseByOneNumberOfInstances(
                    It.IsAny<IContainerConfig>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(0));
        }
    }
}