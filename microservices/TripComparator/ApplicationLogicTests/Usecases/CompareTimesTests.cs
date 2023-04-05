using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TripComparator.External;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class CompareTimesTests
    {
        private CompareTimesUC _compareTimesUc;

        [TestInitialize]
        public void Init()
        {
            _compareTimesUc = new CompareTimesUC(new RouteTimeProviderClient(), new StmClient(null),
                null, null);
        }

        [TestMethod()]
        public async Task TravelTimeBetweenTwoCoordinates()
        {
            var time = await _compareTimesUc.BeginComparingBusAndCarTime("45.49529799006756,-73.56309288413388",
                "45.501735228664714,-73.57656830180076");

            Assert.IsNotNull(time);
        }
    }
}