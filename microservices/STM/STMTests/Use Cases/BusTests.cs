using System.Threading.Tasks;
using ApplicationLogic.Use_Cases;
using Entities.Concretions;
using GTFS;
using GTFS.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STM.Controllers;
using STM.External;
using STMTests.Stub;

namespace STMTests.Use_Cases
{
    [TestClass()]
    public class BusTests
    {
        ItineraryUC _itineraryUc = new ItineraryUC(new StmClient(), new StmData(), null);

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
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 45.495408,
                Longitude = -73.562918
            }, new PositionLL()
            {
                Latitude = 45.501875,
                Longitude = -73.576517
            });

            Assert.IsNotNull(valueTuple?.bus);
            
            Assert.IsNotNull(valueTuple?.bus.Trip.RelevantOrigin?.Index > 0);
            Assert.IsNotNull(valueTuple?.bus.Trip.RelevantDestination?.Index > 0);
        }

        [TestMethod()]
        public async Task GetBusesPeelSherbrookeToOlympicStadium()
        {
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 45.50146231405799,
                Longitude = -73.5769508553735
            }, new PositionLL()
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
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 45.545520,
                Longitude = -73.654904
            }, new PositionLL()
            {
                Latitude = 45.537676,
                Longitude = -73.627113
            });
            
            Assert.IsNotNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task TestFromTimeComparator()
        {
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 45.501784,
                Longitude = -73.576553
            }, new PositionLL()
            {
                Latitude = 45.504731,
                Longitude = -73.573677
            });

            Assert.IsNotNull(valueTuple?.bus);
        }

        [TestMethod()] 
        public async Task GetBusImpossibleTripCoordinate0()
        {
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 0,
                Longitude = 0
            }, new PositionLL()
            {
                Latitude = 0,
                Longitude = 0
            });

            Assert.IsNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleTripCoordinatesNull()
        {
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL(), new PositionLL());

            Assert.IsNull(valueTuple?.bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleWrongCoordinates()
        {
            var valueTuple = await _itineraryUc.GetFastestBus(new PositionLL()
            {
                Latitude = 46.50146231405799,
                Longitude = -73.5769508553735
            }, new PositionLL()
            {
                Latitude = 45.5269499152848,
                Longitude = -73.56423906516093
            });

            Assert.IsNull(valueTuple?.bus);
        }
    }
}