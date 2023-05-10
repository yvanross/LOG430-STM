using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RouteTimeProvider.External;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class TravelTests
    {
        private static readonly CarTravel CarTravel = new();

        [TestMethod()]
        public async Task TravelTimeBetweenTwoCoordinates()
        {
            var time = await CarTravel.GetTravelTimeInSeconds("45.49529799006756,-73.56309288413388",
                "45.501735228664714,-73.57656830180076", new TomTomClient());

            Assert.IsTrue(time > 0);
        }
    }
}