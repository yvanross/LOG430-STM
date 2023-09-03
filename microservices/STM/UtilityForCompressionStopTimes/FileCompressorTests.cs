using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.FileHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileHandlers.Tests
{
    [TestClass()]
    public class FileCompressorTests
    {
        [TestMethod()]
        public async Task CompressTripFileTest()
        {
            await FileCompressor.CompressTripFile(
                @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\stop_times",
                @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\");
        }
    }
}