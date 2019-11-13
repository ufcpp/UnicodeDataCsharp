using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeDataCsharp;

namespace ConsoleApp1
{
    class Program
    {
        const string ucdUrl = "https://www.unicode.org/Public/UCD/latest/ucd/";
        const string cacheFolder = "cache";

        static async Task Main()
        {
            if (!Directory.Exists(cacheFolder))
                Directory.CreateDirectory(cacheFolder);

            await UnicodeData();
            //await GraphemeBreakProperty();
        }

        private static async Task UnicodeData()
        {
            var file = "UnicodeData.txt";

            var content = await Loader.LoadContentAsync(Path.Combine(ucdUrl, file), Path.Combine(cacheFolder, file));

            DecompositionMap(content);
            //ReadUnicodeData(content);
            //ReadLines(content);
        }

        private static async Task GraphemeBreakProperty()
        {
            var file = "GraphemeBreakProperty.txt";

            var content = await Loader.LoadContentAsync(Path.Combine(ucdUrl, "auxiliary", file), Path.Combine(cacheFolder, file));

            foreach (var e in new SingleProperty(content).GetEntries())
            {
                Console.WriteLine(e);
            }

            //CheckInterned(content);
        }

        /// <summary>
        /// <see cref="Parser.GetInternString(ReadOnlyMemory{byte})"/> で作った string は、
        /// x == y なら常に ReferenceEquals(x, y) になってるはず。
        /// というののチェック。
        /// </summary>
        private static void CheckInterned(byte[] content)
        {
            var groups = new SingleProperty(content).GetEntries()
                .Select(x => x.Value)
                .GroupBy(x => x);

            foreach (var g in groups)
            {
                Console.WriteLine(g.Count());

                var first = g.First();
                foreach (var x in g)
                {
                    if (!ReferenceEquals(x, first)) throw new Exception();
                }
            }
        }

        private static void DecompositionMap(byte[] content)
        {
            foreach (var e in new UnicodeData(content).GetEntries())
            {
                if (e.DecompositionMapping.Length == 0) continue;

                Console.WriteLine(e.DecompositionMapping);
            }
        }

        private static void ReadUnicodeData(byte[] content)
        {
            foreach (var e in new UnicodeData(content).GetEntries().Skip(30).Take(50))
            {
                Console.WriteLine(e);
            }
        }

        private static void ReadLines(byte[] content)
        {
            var lines = new LineEnumerable<string, Utf8ToString>(content);

            foreach (var s in lines.Take(10))
            {
                Console.WriteLine(s);
            }
        }

        struct Utf8ToString : ILineFactory<string>
        {
            public string New(ReadOnlyMemory<byte> line) => Encoding.UTF8.GetString(line.Span);
        }
    }
}
