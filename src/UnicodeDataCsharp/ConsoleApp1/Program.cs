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

            var file = "UnicodeData.txt";

            var content = await Loader.LoadContentAsync(Path.Combine(ucdUrl, file), Path.Combine(cacheFolder, file));

            ReadUnicodeData(content);
            //ReadLines(content);
        }

        private static void ReadUnicodeData(byte[] content)
        {
            foreach (var e in new UnicodeData(content).GetEntries().Take(10))
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
