using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApplicationLogic.Usecases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambassador;
using Ambassador.Usecases;

namespace ApplicationLogic.Usecases.Tests
{
    [TestClass()]
    public class TravelTests
    {
        private static readonly RegistrationUC _registration = new();
        
        private static readonly TravelUC _travelUc = new();

        [TestInitialize]
        public void Init()
        {
            _registration.Register(ServiceTypes.RouteTimeProvider.ToString(), default);
        }

        [TestMethod()]
        public async Task TravelTimeBetweenTwoCoordinates()
        {
            var time = await _travelUc.GetTravelTimeInSeconds("45.49529799006756,-73.56309288413388",
                "45.501735228664714,-73.57656830180076");

            Assert.IsTrue(time > 0);
        }
    }
}