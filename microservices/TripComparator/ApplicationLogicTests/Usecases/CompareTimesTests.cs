using Ambassador;
using Ambassador.Controllers;
using Ambassador.Usecases;
using ApplicationLogic.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class CompareTimesTests
    {
        private static readonly CompareTimesUC _compareTimesUc = new();

        [TestInitialize]
        public void Init()
        {
            RegistrationController.Register(ServiceTypes.ComparateurTrajet.ToString(), default);
        }

        [TestMethod()]
        public async Task TravelTimeBetweenTwoCoordinates()
        {
            var time = await _compareTimesUc.CompareBusAndCarTime("45.49529799006756,-73.56309288413388",
                "45.501735228664714,-73.57656830180076");

            Assert.IsTrue(time > 0);
        }
    }
}