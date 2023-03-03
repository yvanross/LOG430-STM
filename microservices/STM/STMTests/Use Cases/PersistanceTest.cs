using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticGTFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using STM.Entities.Concretions;

namespace STM.Use_Cases.Tests
{
    [TestClass()]
    public class PersistenceTest
    {
        Itinary itinary = new Itinary();

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

            await TripCompressor.CompressTripFile(@"C:\Users\david\OneDrive\Documents\stop_times");
        }
    }
}