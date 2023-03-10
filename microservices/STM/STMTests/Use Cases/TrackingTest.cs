using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.Use_Cases;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STM.Controllers;
using STMTests.Stub;

namespace STMTests.Use_Cases
{
    [TestClass()]
    public class TrackingTest
    {
        [TestMethod()]
        public async Task TrackBusesETSToPeelSherbrooke()
        {
            var logger = new Logger<STMOptimalBusController>(new LoggerFactory());
            var logger2 = new Logger<TrackBusController>(new LoggerFactory());

            STMOptimalBusController busController = new STMOptimalBusController(logger);
            
            TrackBusController trackBusController = new TrackBusController(logger2);

            var bus = await busController.Get("45.50146231405799,-73.5769508553735", "45.5269499152848,-73.56423906516093");

            Assert.IsNotNull(bus);

            trackBusController.BeginTracking(bus.Value.First());
        }

        [TestMethod()]
        public async Task TrackBusesPeelSherbrooke()
        {
            var logger = new Logger<STMOptimalBusController>(new LoggerFactory());
            var logger2 = new Logger<TrackBusController>(new LoggerFactory());

            STMOptimalBusController busController = new STMOptimalBusController(logger);

            TrackBusController trackBusController = new TrackBusController(logger2);

            var bus = await busController.Get("45.495408,-73.562918", "45.501875,-73.576517");

            Assert.IsNotNull(bus);

            Assert.IsNotNull(bus);

            trackBusController.BeginTracking(bus.Value.First());
        }

    }
}