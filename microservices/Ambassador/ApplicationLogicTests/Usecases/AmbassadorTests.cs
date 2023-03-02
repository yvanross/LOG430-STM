using Ambassador;
using Ambassador.Usecases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationLogicTests.Usecases
{
    [TestClass()]
    public class AmbassadorTests
    { 
        private readonly RegistrationUC _registrationUc = new ();

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod()]
        public async Task SubscribtionTest()
        {
            await _registrationUc.Register(ServiceTypes.Monitor.ToString(), default);
        }
    }
}