using System.IO.Compression;
using System.Text;

namespace Infrastructure.FileHandlers;

/// <summary>
/// Quality of life to compress the stop_times file provided by the STM (you can delete and handle compression yourself)
/// </summary>
public class FileCompressor
{
    public static async Task CompressTripFile(string filePathWithoutExtension, string pathToDataFolder)
    {
        var lines = await File.ReadAllLinesAsync(filePathWithoutExtension + ".txt");

        var singleLine = string.Join(';', DeleteUselessStrings(lines));

        var byteArray = Encoding.ASCII.GetBytes(singleLine);

        await using var fileStream = new FileStream(pathToDataFolder + "compressed.bin", FileMode.Create);

        await using var zipStream = new GZipStream(fileStream, CompressionMode.Compress, false);

        zipStream.Write(byteArray);
    }

    private static IEnumerable<string> DeleteUselessStrings(string[] lines)
    {
        foreach (var line in lines)
        {
            var lineList = line.Split(',').ToList();

            lineList.RemoveAt(2);

            yield return string.Join(',', lineList);
        }
    }
}