using System.IO.Compression;
using System.Text;

namespace STM;

public class TripCompressor
{
    public static async Task CompressTripFile(string path)
    {
        var lines = await File.ReadAllLinesAsync(path+".txt");

        var singleLine = string.Join(';', DeleteUselessStrings(lines));

        byte[] byteArray = Encoding.ASCII.GetBytes(singleLine);

        await using (FileStream fileStream = new FileStream(path+"compressed.txt", FileMode.Create))
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