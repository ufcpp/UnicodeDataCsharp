using System.IO;
using System.Threading.Tasks;

namespace UnicodeDataCsharp
{
    /// <summary>
    /// <see cref="Loader.LoadContentAsync(string, string)"/> でデータを読んで、
    /// <see cref="UnicodeData"/> とかの型を new して in-memory にキャッシュしておくための型。
    /// </summary>
    public class Ucd
    {
        const string LatestBaseUrl = "https://www.unicode.org/Public/UCD/latest/ucd/";
        const string DefaultCacheFolder = "cache";

        /// <summary>
        /// UCD を取ってくる場所。
        /// <see cref="Loader.LoadContentAsync(string, string)"/> の第1引数の頭に付ける。
        /// null を渡した時は <see cref="LatestBaseUrl"/> を使う。
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// ローカル ストレージのキャッシュ用フォルダー。
        /// <see cref="Loader.LoadContentAsync(string, string)"/> の第2引数の頭に付ける。
        /// null を渡した時は <see cref="DefaultCacheFolder"/> を使う。
        /// </summary>
        public string CacheFolder { get; }

        /// <summary>
        /// </summary>
        /// <param name="url"><see cref="BaseUrl"/></param>
        /// <param name="cacheFolder"><see cref="CacheFolder"/></param>
        public Ucd(string url = null, string cacheFolder = null)
        {
            BaseUrl = url ?? LatestBaseUrl;
            CacheFolder = cacheFolder ?? DefaultCacheFolder;

            if (!Directory.Exists(CacheFolder))
                Directory.CreateDirectory(CacheFolder);
        }

        public async ValueTask<UnicodeData> GetUnicodeData()
            => _UnicodeData ??=
                new UnicodeData(
                    await Loader.LoadContentAsync(
                        Path.Combine(BaseUrl, "UnicodeData.Txt"),
                        Path.Combine(CacheFolder, "UnicodeData.Txt")));

        private UnicodeData? _UnicodeData;

        public async ValueTask<SingleProperty> GetGraphemeBreakProperty()
            => _GraphemeBreakProperty ??=
                new SingleProperty(
                    await Loader.LoadContentAsync(
                        Path.Combine(BaseUrl, "auxiliary", "GraphemeBreakProperty.Txt"),
                        Path.Combine(CacheFolder, "GraphemeBreakProperty.Txt")));

        private SingleProperty? _GraphemeBreakProperty;
    }
}
