using Infrastructure.FileHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Not_A_Test_But_Utility_See_Doc;

[TestClass]
public class FileCompressorTests
{
    /// <summary>
    /// This is marked as a test for convenience but it is really just a utility method
    /// It isn't part of the main application and is only used to compress the stop_times.txt to bin
    /// Since i didn't want to create a script, here it is.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task CompressTripFileTest()
    {
        await FileCompressor.CompressTripFile(
            @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\stop_times",
            @"C:\Users\david\Documents\GitHub\LOG430-STM\microservices\STM\Aspect.Configuration\Data\");
    }
}