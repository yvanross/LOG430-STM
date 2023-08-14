﻿using System.Threading.Tasks;
using ApplicationLogic.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STMTests.Use_Cases
{
    [TestClass()]
    public class BusTests
    {
        /*
        Itinerary _itinerary = new Itinerary(new StmClient(), new TransitDataCache(), null);

        [TestMethod()]
        public async Task TestController()
        {
            FinderController busController = new FinderController(new Logger<FinderController>(new LoggerFactory()));

            //var bus = await busController.Get("45.50146231405799,-73.5769508553735", "45.5269499152848,-73.56423906516093");
            
            //Assert.IsNotNull(bus);
        }

        [TestMethod()]
        public async Task GetBusesETSToPeelSherbrooke()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 45.495408,
                Longitude = -73.562918
            }, new PositionLL()
            {
                Latitude = 45.501875,
                Longitude = -73.576517
            });

            Assert.IsNotNull(valueTuple?.First().bus);
            
            Assert.IsNotNull(valueTuple?.First().bus.Ride.RelevantOrigin?.Index > 0);
            Assert.IsNotNull(valueTuple?.First().bus.Ride.RelevantDestination?.Index > 0);
        }

        [TestMethod()]
        public async Task GetBusesPeelSherbrookeToOlympicStadium()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 45.50146231405799,
                Longitude = -73.5769508553735
            }, new PositionLL()
            {
                Latitude = 45.5269499152848,
                Longitude = -73.56423906516093
            });

            Assert.IsNotNull(valueTuple?.First().bus);

            Assert.IsTrue(valueTuple.First().bus.Name.Equals("24") || valueTuple.First().bus.Name.Equals("356"));
        }

        [TestMethod()]
        public async Task GetBusesSTLaurentToJarryPark()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 45.545520,
                Longitude = -73.654904
            }, new PositionLL()
            {
                Latitude = 45.537676,
                Longitude = -73.627113
            });
            
            Assert.IsNotNull(valueTuple?.First().bus);
        }

        [TestMethod()]
        public async Task GetBusesReneLevesque()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 45.49739613164703,
                Longitude = -73.57152386654649
            }, new PositionLL()
            {
                Latitude = 45.49999809124717,
                Longitude = -73.56902941233237
            });

            Assert.IsNotNull(valueTuple?.First().bus);
        }

        [TestMethod()]
        public async Task GetAvenueDuParc()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 45.51199388170343,
                Longitude = -73.57865273764862
            }, new PositionLL()
            {
                Latitude = 45.51684531041917,
                Longitude = -73.58915451596351
            });

            Assert.IsNotNull(valueTuple?.First().bus);
        }

        [TestMethod()] 
        public async Task GetBusImpossibleTripCoordinate()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 0,
                Longitude = 0
            }, new PositionLL()
            {
                Latitude = 0,
                Longitude = 0
            });

            Assert.IsNull(valueTuple?.First().bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleTripCoordinatesNull()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL(), new PositionLL());

            Assert.IsNull(valueTuple?.First().bus);
        }

        [TestMethod()]
        public async Task GetBusImpossibleWrongCoordinates()
        {
            var valueTuple = await _itinerary.GetFastestBus(new PositionLL()
            {
                Latitude = 46.50146231405799,
                Longitude = -73.5769508553735
            }, new PositionLL()
            {
                Latitude = 45.5269499152848,
                Longitude = -73.56423906516093
            });

            Assert.IsNull(valueTuple?.First().bus);
        }
        */
        
        [TestMethod()]
        public async Task CompressTripFile()
        {
            await FileCompressor.CompressTripFile(@"C:\Users\david\Downloads\GTFS_Update\stop_times",
                @"C:\Users\david\Downloads\GTFS_Update\");
        }
    }
}