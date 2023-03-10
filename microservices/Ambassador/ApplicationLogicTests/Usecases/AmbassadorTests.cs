using Ambassador;
using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.Controllers;
using Ambassador.Health;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class AmbassadorTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod()]
        public async Task SubscriptionTest()
        {
            await RegistrationController.Register("test", default);
        }

        [TestMethod()]
        public async Task GetTest()
        {
            await RegistrationController.Register("test", default);
        }
    }
}