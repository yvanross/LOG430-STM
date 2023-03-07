using System.Threading.Tasks;
using ApplicationLogic.Use_Cases;
using GTFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STMTests.Stub;

namespace STMTests.Use_Cases
{
    [TestClass()]
    public class PersistenceTest
    {
        ItineraryUC _itinerary = new (null, new StmDataStub(), null);

        [TestMethod()]
        public void TestStaticGTFS()
        {
            var infos = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES);
            
            Assert.IsNotNull(infos);
        }

        [TestMethod()]
        public void TestStaticGTFSAnswerSpeed()
        {
            var infos = DynamicStaticGTFSParser.GetInfo(DataCategory.STOP_TIMES);

            Assert.IsNotNull(infos);
        }

        [TestMethod()]
        public async Task CreateNewCompressedFile()
        {
            bool run = false;

            if (!run) return;

            await FileCompressor.CompressTripFile(@"C:\Users\david\OneDrive\Documents\stop_times");
        }
    }
}