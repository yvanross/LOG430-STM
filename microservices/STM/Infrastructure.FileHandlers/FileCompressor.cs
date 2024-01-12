using System.IO.Compression;
using System.Text;

namespace Infrastructure.FileHandlers;

/// <summary>
///     Quality of life to compress the stop_times file provided by the STM (you can delete and handle compression
///     yourself)
/// </summary>
public class FileCompressor
{
    public static async Task CompressStopTimesFile(string filePathWithoutExtension, string pathToDataFolder)
    {
        await CleanAndCompressData(filePathWithoutExtension, pathToDataFolder + "stop_times.bin", [0, 2, 3, 4]);
    }

    public static async Task CompressTripsFile(string filePathWithoutExtension, string pathToDataFolder)
    {
        await CleanAndCompressData(filePathWithoutExtension, pathToDataFolder + "trips.bin", [2]);
    }

    public static async Task CompressStopsFile(string filePathWithoutExtension, string pathToDataFolder)
    {
        await CleanAndCompressData(filePathWithoutExtension, pathToDataFolder + "stops.bin", [0, 3, 4]);
    }

    private static async Task CleanAndCompressData(string filePathWithoutExtension, string pathToDataFolder, HashSet<int> indexesToKeep)
    {
        var lines = await File.ReadAllLinesAsync(filePathWithoutExtension + ".txt");

        var newData = DeleteUselessStrings(lines, indexesToKeep).ToList();

        CompressData(newData, pathToDataFolder);
    }

    private static IEnumerable<string> DeleteUselessStrings(string[] lines, HashSet<int> indexesToKeep)
    {
        foreach (var line in lines)
        {
            var lineList = line.Split(',').ToList();

            yield return string.Join(',', lineList.Where((_, index) => indexesToKeep.Contains(index)));
        }
    }

    private static void CompressData(List<string> data, string destinationFilePath)
    {
        using var fileStream = new FileStream(destinationFilePath, FileMode.Create);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        using var writer = new StreamWriter(gzipStream, Encoding.UTF8);
       
        foreach (var line in data)
        {
            writer.WriteLine(line);
        }
    }
}