using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.Live;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Monitor = ApplicationLogic.Usecases.Monitor;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class MonitorTest
    {
        private Monitor _monitor;

        private Mock<IEnvironmentClient> _envMock;

        private Mock<IPodReadService> _podReadModel;

        [TestInitialize]
        public void Init()
        {
            _envMock = MockProvider.GetEnvironmentMock();
            _podReadModel = MockProvider.GetReadModelMock();

            _monitor = new Monitor(
                _envMock.Object,
                _podReadModel.Object,
                MockProvider.GetWriteModelMock().Object);
        }

        [TestMethod()]
        public async Task ProcessPodStates_PodsNeedToBeRemoved()
        {
            _envMock.Setup(x => x.GetRunningServices(It.IsAny<string[]>())).Returns(Task.FromResult(ImmutableList<string>.Empty)!);

            await _monitor.RemoveOrReplaceDeadPodsFromModel();

            _envMock.Verify(x=>x.RemoveContainerInstance(
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(6));
        }

        [TestMethod()]
        public async Task ProcessPodStates_NoPodsNeedToBeRemoved()
        {
            await _monitor.RemoveOrReplaceDeadPodsFromModel();

            _envMock.Verify(x => x.RemoveContainerInstance(
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(0));
        }

        [TestMethod()]
        public async Task GarbageCollection()
        {
            await _monitor.GarbageCollection();
        }

        [TestMethod()]
        public async Task MatchDemandOnPods_NeedsMorePods()
        {
            _podReadModel.Setup(x => x.GetPodInstances(It.IsAny<string>())).Returns(ImmutableList<IPodInstance>.Empty);

            await _monitor.MatchInstanceDemandOnPods();

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
            await _monitor.MatchInstanceDemandOnPods();

            _envMock.Verify(x => x.IncreaseByOneNumberOfInstances(
                    It.IsAny<IContainerConfig>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(0));
        }
    }
}