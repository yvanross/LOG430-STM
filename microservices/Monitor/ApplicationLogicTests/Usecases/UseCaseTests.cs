using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitor.Docker;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class UseCaseTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod()]
        public async Task GetRunningMicroservices()
        {
            MonitorUc monitor = new MonitorUc(new LocalDockerClient());

            var microservices = await monitor.GetRunningMicroservices();

            Assert.IsNotNull(microservices);
        }
    }
}