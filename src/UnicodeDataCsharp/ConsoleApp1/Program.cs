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
            var ucd = new Ucd();

            await GraphemeBreakProperty(ucd);
            await UnicodeData(ucd);
        }

        private static async Task UnicodeData(Ucd ucd)
        {
            var ud = await ucd.GetUnicodeData();

            DecompositionMap(ud);
            ReadUnicodeData(ud);
        }

        private static async Task GraphemeBreakProperty(Ucd ucd)
        {
            var gb = await ucd.GetGraphemeBreakProperty();

            foreach (var e in gb.GetEntries())
            {
                Console.WriteLine(e);
            }

            CheckInterned(gb);
        }

        /// <summary>
        /// <see cref="Parser.GetInternString(ReadOnlyMemory{byte})"/> で作った string は、
        /// x == y なら常に ReferenceEquals(x, y) になってるはず。
        /// というののチェック。
        /// </summary>
        private static void CheckInterned(SingleProperty sp)
        {
            var groups = sp.GetEntries()
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

        private static void DecompositionMap(UnicodeData ud)
        {
            foreach (var e in ud.GetEntries())
            {
                if (e.DecompositionMapping.Length == 0) continue;

                Console.WriteLine(e.DecompositionMapping);
            }
        }

        private static void ReadUnicodeData(UnicodeData ud)
        {
            foreach (var e in ud.GetEntries().Skip(30).Take(50))
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
