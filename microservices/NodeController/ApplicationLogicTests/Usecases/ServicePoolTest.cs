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
    public class ServicePoolTest
    {
        private ServicePoolDiscoveryUC servicePoolDiscoveryUC;
      
        private Mock<IEnvironmentClient> _envMock;

        private Mock<IPodReadModel> _readModelMock;

        [TestInitialize]
        public void Init()
        {
            _envMock = MockProvider.GetEnvironmentMock();
            _readModelMock = MockProvider.GetReadModelMock();

            servicePoolDiscoveryUC = new ServicePoolDiscoveryUC(
                MockProvider.GetWriteModelMock().Object,
                _readModelMock.Object,
                _envMock.Object);
        }

        [TestMethod()]
        public async Task DiscoverServices()
        {
            _readModelMock.Setup(x => x.GetAllServices()).Returns(ImmutableList<IServiceInstance>.Empty);

            await servicePoolDiscoveryUC.DiscoverServices();
        }
    }
}