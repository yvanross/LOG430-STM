using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticGTFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Entities.Concretions;
using Microsoft.Extensions.Logging;
using STM.Controllers;
using STM.Entities.Concretions;
using STMTests;

namespace STM.Use_Cases.Tests
{
    [TestClass()]
    public class TrackingTest
    {
        Itinary itinary = new Itinary();

        [TestMethod()]
        public async Task TrackBusesETSToPeelSherbrooke()
        {
            var logger = new Logger<STMOptimalBusController>(new LoggerFactory());

            STMOptimalBusController busController = new STMOptimalBusController(logger);

            var bus = await busController.Get("45.50146231405799,-73.5769508553735", "45.5269499152848,-73.56423906516093");

            Assert.IsNotNull(bus);

            TrackBus track = new TrackBus(bus,logger);

            await track.PerdiodicCaller();

            Console.WriteLine();
        }

        [TestMethod()]
        public async Task TrackBusesPeelSherbrooke()
        {
            var logger = new Logger<STMOptimalBusController>(new LoggerFactory());

            STMOptimalBusController busController = new STMOptimalBusController(logger);

            var bus = await busController.Get("45.495408,-73.562918", "45.501875,-73.576517");

            bus.callBack = "/BusTrackingResult";
            bus.processID = "1111";

            Assert.IsNotNull(bus);

            TrackBus track = new TrackBus(bus,
                logger);

            await track.PerdiodicCaller();

            Console.WriteLine();
        }

    }
}