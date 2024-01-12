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
        await FileCompressor.CompressStopTimesFile(
            @"..\..\..\..\Configuration\Data\stop_times",
            @"..\..\..\..\Configuration\Data\");

        await FileCompressor.CompressTripsFile(
            @"..\..\..\..\Configuration\Data\trips",
            @"..\..\..\..\Configuration\Data\");

        await FileCompressor.CompressStopsFile(
            @"..\..\..\..\Configuration\Data\stops",
            @"..\..\..\..\Configuration\Data\");
    }
}