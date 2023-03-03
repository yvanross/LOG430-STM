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

namespace STM.Use_Cases.Tests
{
    [TestClass()]
    public class BusTests
    {
        Itinary itinary = new Itinary();



        [TestInitialize]
        public void Setup()
        {
            if(STMData.Trips is null)
                STMData.PrefetchData();
        }

        [TestMethod()]
        public async Task TestController()
        {
            STMOptimalBusController busController = new STMOptimalBusController(new Logger<STMOptimalBusController>(new LoggerFactory()));

            var bus = await busController.Get("45.50146231405799,-73.5769508553735", "45.5269499152848,-73.56423906516093");
            
            Assert.IsNotNull(bus);
        }

        [TestMethod()]
        public async Task GetBusesETSToPeelSherbrooke()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 45.495408,
                Longitude = -73.562918
            }, new Position()
            {
                Latitude = 45.501875,
                Longitude = -73.576517
            });

            Assert.IsNotNull(valueTuple?.bus);
            
            Assert.IsNotNull(valueTuple?.bus.Trip.RelevantOrigin?.index > 0);
            Assert.IsNotNull(valueTuple?.bus.Trip.RelevantDestination?.index > 0);
        }

        [TestMethod()]
        public async Task GetBusesPeelSherbrookeToOlympicStadium()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 45.50146231405799,
                Longitude = -73.5769508553735
            }, new Position()
            {
                Latitude = 45.5269499152848,
                Longitude = -73.56423906516093
            });

            Assert.IsNotNull(valueTuple?.bus);

            Assert.IsTrue(valueTuple.Value.bus.Name.Equals("24") || valueTuple.Value.bus.Name.Equals("356"));
        }

        [TestMethod()]
        public async Task GetBusesSTLaurentToJarryPark()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 45.545520,
                Longitude = -73.654904
            }, new Position()
            {
                Latitude = 45.537676,
                Longitude = -73.627113
            });
            
            Assert.IsNotNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task TestFromTimeComparator()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 45.501784,
                Longitude = -73.576553
            }, new Position()
            {
                Latitude = 45.504731,
                Longitude = -73.573677
            });

            Assert.IsNotNull(valueTuple?.bus);
        }

        [TestMethod()] 
        public async Task GetBusImpossibleTripCoordinate0()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 0,
                Longitude = 0
            }, new Position()
            {
                Latitude = 0,
                Longitude = 0
            });

            Assert.IsNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleTripCoordinatesNull()
        {
            var valueTuple = await itinary.GetFastestBus(new Position(), new Position());

            Assert.IsNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleWrongCoordinates()
        {
            var valueTuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = 46.50146231405799,
                Longitude = -73.5769508553735
            }, new Position()
            {
                Latitude = 45.5269499152848,
                Longitude = -73.56423906516093
            });

            Assert.IsNull(valueTuple?.bus);
        }
    }
}