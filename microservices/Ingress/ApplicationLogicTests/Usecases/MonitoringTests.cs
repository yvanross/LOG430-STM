using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitor.Docker;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class MonitoringTests
    {
        [TestInitialize]
        public void Init()
        {

        }
        /*
        [TestMethod()]
        public async Task GetRunningMicroservices()
        {
            MonitorUc monitor = new MonitorUc(new LocalDockerClient());

            var microservices = await monitor.GetRunningMicroservices();

            Assert.IsNotNull(microservices);
        }

        [TestMethod()]
        public async Task StartAndStopContainers()
        {
            MonitorUc monitor = new MonitorUc(new LocalDockerClient());

            var initialMicroservices = await monitor.GetRunningMicroservices();

            var microserviceToLoadBalance = initialMicroservices.First();

            await monitor.IncreaseByOneNumberOfInstances(microserviceToLoadBalance.Id, microserviceToLoadBalance.Name + "-new");

            Thread.Sleep(1000);

            var increasedMicroservices = await monitor.GetRunningMicroservices();

            var newMicroservice = increasedMicroservices.Single(s=>s.Name.Equals(microserviceToLoadBalance.Name + "-new") && s.ImageName.Equals(microserviceToLoadBalance.ImageName));

            await monitor.RemoveContainerInstance(newMicroservice.Id);

            var finalStateMicroservices = await monitor.GetRunningMicroservices();

            Assert.IsFalse(finalStateMicroservices.Any(s => s.Name.Equals(microserviceToLoadBalance.Name + "-new") && s.ImageName.Equals(microserviceToLoadBalance.ImageName)));
        }
        */
    }
}