using System.IO.Compression;
using System.Text;

namespace GTFS;

public class FileCompressor
{
    public static async Task CompressTripFile(string path)
    {
        var lines = await File.ReadAllLinesAsync(path+".txt");

        var singleLine = string.Join(';', DeleteUselessStrings(lines));

        byte[] byteArray = Encoding.ASCII.GetBytes(singleLine);

        await using (FileStream fileStream = new FileStream(@"C:\Users\david\Documents\GitHub\LOG430-STM-Template\microservices\STM\StaticGTFS\Resources" + "compressed.bin", FileMode.Create))
        {
            await using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Compress, false))
            {
                zipStream.Write(byteArray);
            }
        }
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