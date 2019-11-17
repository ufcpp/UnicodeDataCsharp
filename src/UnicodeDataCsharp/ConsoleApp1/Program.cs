using System;
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

            await UnicodeData(ucd);
        }

        private static async Task UnicodeData(Ucd ucd)
        {
            var ud = await ucd.GetUnicodeData();

            DecompositionMap(ud);
            ReadUnicodeData(ud);
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
