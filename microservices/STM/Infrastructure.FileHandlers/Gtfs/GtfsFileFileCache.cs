using System.IO.Compression;
using System.Text;

namespace Infrastructure.FileHandlers.Gtfs;

public class GtfsFileFileCache : IDisposable
{
    private readonly IDataReader _dataReader;

    private static readonly Dictionary<DataCategoryEnum, GtfsInfo[]> _GtfsInfos = new();

    private bool _disposed;

    public GtfsFileFileCache(IDataReader dataReader)
    {
        _dataReader = dataReader;
    }

    public void LoadFileCache()
    {
        FetchStopTimes();

        FetchStringData(DataCategoryEnum.TRIPS);

        FetchStringData(DataCategoryEnum.STOPS);
    }

    public GtfsInfo[] GetInfo(DataCategoryEnum dataCategory)
    {
        if (_GtfsInfos.TryGetValue(dataCategory, out var infos) is false)
            throw new ArgumentException($"No data found for {dataCategory}");

        return infos;
    }

    private void FetchStringData(DataCategoryEnum dataCategory)
    {
        var data = _dataReader.GetString(dataCategory.ToString().ToLower());

        ParseGtfs(dataCategory, data);
    }

    private void FetchStopTimes()
    {
        var stopTimes = _dataReader.GetObject(DataCategoryEnum.STOP_TIMES.ToString().ToLower());

        ParseGtfs(DataCategoryEnum.STOP_TIMES, DecompressStopTimes(stopTimes));
    }

    private string DecompressStopTimes(object resource)
    {
        string decompressed;

        var encodedBytes = (byte[])resource;

        using var inGoingStream = new MemoryStream(encodedBytes);
        using var outGoingStream = new MemoryStream();
        using (var gZipStream = new GZipStream(inGoingStream, CompressionMode.Decompress))
        {
            CopyTo(gZipStream, outGoingStream);
        }

        decompressed = Encoding.UTF8.GetString(outGoingStream.ToArray()).Replace(";", "\n");

        return decompressed;
    }

    private void ParseGtfs(DataCategoryEnum dataCategory, string text)
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

    public void Dispose()
    {
        if (_disposed is false)
        {
            foreach (var item in _GtfsInfos.Values)
            {
                foreach (var gtfsInfo in item)
                {
                    gtfsInfo.Dispose();
                }
            }

            _GtfsInfos.Clear();

            _disposed = true;
        }

    }
}