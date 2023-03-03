using System.IO.Compression;
using System.Text;

namespace StaticGTFS;

public static class DynamicStaticGTFSParser
{
    private static Dictionary<DataCategory, GTFSInfo[]> _GTFSInfos = new ();

    static DynamicStaticGTFSParser()
    {
        FetchStopTimes();

        FetchStringData(DataCategory.TRIPS);
       
        FetchStringData(DataCategory.STOPS);
    }

    public static GTFSInfo[]? GetInfo(DataCategory dataCategory)
    {
        _GTFSInfos.TryGetValue(dataCategory, out var infos);

        return infos;
    }

    private static void FetchStringData(DataCategory dataCategory)
    {
        var data =
            StaticGTFS.Properties.Resources.ResourceManager.GetString(dataCategory.ToString().ToLower());

        ParseGTFS(dataCategory, data);
    }

    private static void FetchStopTimes()
    {
        var stopTimes =
            StaticGTFS.Properties.Resources.ResourceManager.GetObject(DataCategory.STOP_TIMES.ToString().ToLower());

        ParseGTFS(DataCategory.STOP_TIMES, DecompressStopTimes(stopTimes));
    }

    private static string DecompressStopTimes(object? ressoruce)
    {
        string decompressed;

        var encodedBytes = (byte[]) ressoruce;

        using (var inGoingStream = new MemoryStream(encodedBytes))
        using (var outGoingStream = new MemoryStream())
        {
            using (var gZipStream = new GZipStream(inGoingStream, CompressionMode.Decompress))
            {
                CopyTo(gZipStream, outGoingStream);
            }

            decompressed = Encoding.UTF8.GetString(outGoingStream.ToArray()).Replace(";", "\n");
        }

        return decompressed;
    }

    private static void ParseGTFS(DataCategory dataCategory, string text)
    {
        var rows = text.Split("\n");

        if (_GTFSInfos.TryAdd(dataCategory, new GTFSInfo[rows.Length - 1]) &&
            _GTFSInfos.TryGetValue(dataCategory, out var gtfsInfo))
        {
            var tags = rows[0].Split(',');

            for (int i = 1; i < rows.Length; i++)
            {
                var fields = rows[i].Split(',');

                for (int j = 0; j < fields.Length; j++)
                {
                    gtfsInfo[i - 1] ??= new GTFSInfo();

                    gtfsInfo[i - 1].info.Add(tags[j], fields[j]);
                }
            }
        }
    }

    private static void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }
}