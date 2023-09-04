using Infrastructure.FileHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UtilityForCompressionStopTimes;

[TestClass]
public class FileCompressorTests
{
    [TestMethod]
    public async Task CompressTripFileTest()
    {
        await FileCompressor.CompressTripFile(
            @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\stop_times",
            @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\");
    }
}