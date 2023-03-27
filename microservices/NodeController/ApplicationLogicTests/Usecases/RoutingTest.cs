using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class RoutingTest
    {
        private RoutingUC routingUc;

        [TestInitialize]
        public void Init()
        {
            routingUc = new RoutingUC(MockProvider.GetReadModelMock().Object);
        }

        [TestMethod()]
        public void LoadBalancing_Broadcast()
        {
            var dest= routingUc.LoadBalancing(
                MockProvider.GetReadModelMock().Object.GetAllServices().ToList(),
                LoadBalancingMode.Broadcast).ToList();

            dest.Should().HaveCountGreaterThan(1);
        }

        [TestMethod()]
        public void LoadBalancing_RoundRobin()
        {
            var dest = routingUc.LoadBalancing(
                MockProvider.GetReadModelMock().Object.GetAllServices().ToList(),
                LoadBalancingMode.RoundRobin).ToList();

            dest.Should().HaveCount(1);
        }

        [TestMethod()]
        public void RouteByDestinationType_RoundRobin()
        {
            var source = MockProvider.GetReadModelMock().Object.GetServiceInstances("doesnt_matter_its_a_mock").First();

            var dest = routingUc.RouteByDestinationType(source.Id,
                MockProvider.GetReadModelMock().Object.GetServiceType("doesnt_matter_its_a_mock").Type,
                LoadBalancingMode.RoundRobin).ToList();

            dest.Should().HaveCount(1);

            dest.First().Address.Should().NotBeEquivalentTo(source.HttpRoute);
        }

        [TestMethod()]
        public void RouteByDestinationType_BroadCast()
        {
            var source = MockProvider.GetReadModelMock().Object.GetServiceInstances("doesnt_matter_its_a_mock").First();

            var dest = routingUc.RouteByDestinationType(source.Id,
                MockProvider.GetReadModelMock().Object.GetServiceType("doesnt_matter_its_a_mock").Type,
                LoadBalancingMode.Broadcast).ToList();

            dest.Should().AllSatisfy(x => x.Address.Should().NotBeEquivalentTo(source.HttpRoute));
        }
    }
}