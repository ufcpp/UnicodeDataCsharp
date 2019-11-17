using System.Linq;
using System.Text;
using UnicodeDataCsharp;
using Xunit;

namespace UnicodeDataCsharpTest
{
    public class MapTest
    {
        private static readonly Encoding utf8 = Encoding.UTF8;

        [Fact]
        public void GetString()
        {
            var data = new[]
            {
                utf8.GetBytes("abc"),
                utf8.GetBytes("xyz"),
                utf8.GetBytes("abcdefghi"),
                utf8.GetBytes("123456789"),
                utf8.GetBytes("αβγ"),
                utf8.GetBytes("あいう"),
                utf8.GetBytes("一二三"),
                utf8.GetBytes("🤣😂😊❤😍😒👌😘"),
            };

            var map = new Map();

            var internedStrings = data.Select(b => map.GetString(b)).ToArray();

            for (int i = 0; i < 100; i++)
            {
                var utf8 = data[i % data.Length];
                var str = map.GetString(utf8);

                Assert.Contains(internedStrings, x => ReferenceEquals(x, str));
            }

            Assert.Equal(data.Length, map.Count);
        }

        [Fact]
        public void GetInternedString()
        {
            var data = new[]
            {
                "abc",
                "xyz",
                "abcdefghi",
                "123456789",
                "αβγ",
                "あいう",
                "一二三",
                "🤣😂😊❤😍😒👌😘",
            };

            var map = new Map();

            var internedStrings = data.Select(s => map.GetString(s)).ToArray();

            for (int i = 0; i < 100; i++)
            {
                var str = data[i % data.Length];
                var clone = (string)str.Clone();
                var interned = map.GetString(clone);

                Assert.Contains(internedStrings, x => ReferenceEquals(x, interned));
                Assert.True(internedStrings.All(x => !ReferenceEquals(x, clone)));
            }

            Assert.Equal(data.Length, map.Count);
        }
    }
}
