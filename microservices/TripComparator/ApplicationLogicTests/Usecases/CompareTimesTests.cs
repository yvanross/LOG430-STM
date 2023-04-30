using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class CompareTimesTests
    {
        private CompareTimes _compareTimes;

        [TestInitialize]
        public void Init()
        {
            //_compareTimes = new CompareTimes(new RouteTimeProviderClient(), new StmClient(null),
            //    null, null);
        }

        [TestMethod()]
        public async Task TravelTimeBetweenTwoCoordinates()
        {
            var time = await _compareTimes.BeginComparingBusAndCarTime("45.49529799006756,-73.56309288413388",
                "45.501735228664714,-73.57656830180076");

            Assert.IsNotNull(time);
        }
    }
}