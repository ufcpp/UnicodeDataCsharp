using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
        public Ucd(string? url = null, string? cacheFolder = null)
        {
            BaseUrl = url ?? LatestBaseUrl;
            CacheFolder = cacheFolder ?? DefaultCacheFolder;

            if (!Directory.Exists(CacheFolder))
                Directory.CreateDirectory(CacheFolder);
        }

        private Task<byte[]> LoadAsync(string name, string? subFolder = null)
        {
            var relative = name;
            if (subFolder is { } f) relative = f + "/" + relative;

            return Loader.LoadContentAsync(
                BaseUrl + relative,
                Path.Combine(CacheFolder, name));
        }

        public async ValueTask<UnicodeData> GetUnicodeData()
            => _UnicodeData ??= new UnicodeData(await LoadAsync("UnicodeData.txt"));
        private UnicodeData? _UnicodeData;

        // ReadOnlyMemory で返すべきか迷う。
        // 配列返すと書き換えられちゃうからほんとはまずいけど、生配列の方がパフォーマンスがよく。
        public async ValueTask<UnicodeData.Entry[]> GetUnicodeDataEntries()
            => _UnicodeDataEntries ??= (await GetUnicodeData()).GetEntries().ToArray();
        private UnicodeData.Entry[]? _UnicodeDataEntries;

        public async ValueTask<SingleProperty> GetGraphemeBreakProperty()
            => _GraphemeBreakProperty ??= new SingleProperty(await LoadAsync("GraphemeBreakProperty.txt", "auxiliary"));

        private SingleProperty? _GraphemeBreakProperty;

        public async ValueTask<SingleProperty.Entry[]> GetGraphemeBreakPropertyEntries()
            => _GraphemeBreakPropertyEntries ??= (await GetGraphemeBreakProperty()).GetEntries().ToArray();
        private SingleProperty.Entry[]? _GraphemeBreakPropertyEntries;
    }
}
