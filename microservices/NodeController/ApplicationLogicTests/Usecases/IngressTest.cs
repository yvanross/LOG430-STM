using ApplicationLogic.Usecases;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class IngressTest
    {
        private Ingress _ingress;

        [TestInitialize]
        public void Init()
        {
            _ingress = new Ingress(MockProvider.GetHostInfoMock().Object,
                MockProvider.GetIngressClientMock().Object);
        }

        [TestMethod()]
        public async Task SubscriptionTest()
        {
            await _ingress.Register();
        }

        [TestMethod()]
        public async Task GetLogStoreAddressAndPort()
        {
            var address = await _ingress.GetLogStoreAddressAndPort();

            address.Should().BeEquivalentTo("http://testAddress:8329");
        }
    }
}