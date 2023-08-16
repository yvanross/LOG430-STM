using System.IO.Compression;
using System.Resources;
using System.Text;
using ApplicationLogic.Enums;
using Infrastructure2.Gtfs;

namespace Infrastructure2.Cache;

public class GtfsFileFileCache
{
    private readonly ResourceManager _resourceManager;

    private static readonly Dictionary<DataCategoryEnum, GtfsInfo[]> _GtfsInfos = new ();

    public GtfsFileFileCache(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;

        FetchStopTimes();

        FetchStringData(DataCategoryEnum.TRIPS);
       
        FetchStringData(DataCategoryEnum.STOPS);
    }

    public void FlushData()
    {
        _GtfsInfos.Clear();
    }

    public GtfsInfo[]? GetInfo(DataCategoryEnum dataCategory)
    {
        _GtfsInfos.TryGetValue(dataCategory, out var infos);

        return infos;
    }

    private void FetchStringData(DataCategoryEnum dataCategory)
    {
        var data = _resourceManager.GetString(dataCategory.ToString().ToLower());

        ParseGTFS(dataCategory, data);
    }

    private void FetchStopTimes()
    {
        var stopTimes = _resourceManager.GetObject(DataCategoryEnum.STOP_TIMES.ToString().ToLower());

        ParseGTFS(DataCategoryEnum.STOP_TIMES, DecompressStopTimes(stopTimes));
    }

    private string DecompressStopTimes(object? ressoruce)
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

    private void ParseGTFS(DataCategoryEnum dataCategory, string text)
    {
        var rows = text.Split("\n");

        if (_GtfsInfos.TryAdd(dataCategory, new GtfsInfo[rows.Length - 1]) &&
            _GtfsInfos.TryGetValue(dataCategory, out var gtfsInfo))
        {
            var tags = rows[0].Split(',');

            for (int i = 1; i < rows.Length; i++)
            {
                var fields = rows[i].Split(',');

                for (int j = 0; j < fields.Length; j++)
                {
                    gtfsInfo[i - 1] ??= new GtfsInfo();

                    gtfsInfo[i - 1].Info.Add(tags[j], fields[j]);
                }
            }
        }
    }

    private void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }
}