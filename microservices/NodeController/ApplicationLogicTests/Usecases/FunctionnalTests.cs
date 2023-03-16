using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class FunctionnalTests
    {
        /*
        private readonly RoutingUC _routingUc = new (new RepositoryRead());
        
        private readonly SubscriptionUC _subscriptionUc = new (new RepositoryWrite(), new RepositoryRead());

        private string _serviceType = "testServiceType";

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod()]
        public void SubscriptionTest()
        {
            _subscriptionUc.Subscribe("0.0.0.0", "0", _serviceType);

            _subscriptionUc.Subscribe("0.0.0.0", "0", _serviceType);

            Assert.IsTrue(_subscriptionUc.CheckIfServiceIsSubscribed("0.0.0.0", "0"));
        }

        [TestMethod()]
        public void RoutingTest()
        {
            _subscriptionUc.Subscribe("0.0.0.0", "0", _serviceType);

            Assert.IsTrue(_routingUc.RouteByDestinationType(_serviceType).Equals("http://0.0.0.0:0"));
        }
        */
    }
}