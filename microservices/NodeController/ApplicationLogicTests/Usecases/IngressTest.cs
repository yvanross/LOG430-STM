using ApplicationLogic.Usecases;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class IngressTest
    {
        private IngressUC _ingressUc;

        [TestInitialize]
        public void Init()
        {
            _ingressUc = new IngressUC(MockProvider.GetHostInfoMock().Object,
                MockProvider.GetIngressClientMock().Object);
        }

        [TestMethod()]
        public async Task SubscriptionTest()
        {
            await _ingressUc.Register();
        }

        [TestMethod()]
        public async Task GetLogStoreAddressAndPort()
        {
            var address = await _ingressUc.GetLogStoreAddressAndPort();

            address.Should().BeEquivalentTo("http://testAddress:8329");
        }
    }
}