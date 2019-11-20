using System;
using System.Linq;
using System.Threading.Tasks;
using UnicodeDataCsharp;
using Xunit;

namespace UnicodeDataCsharpTest
{
    public class UcdTest
    {
        public static Ucd Ucd { get; } = new Ucd();

        [Fact]
        public async Task CheckInterned()
        {
            var gb = await Ucd.GetGraphemeBreakProperty();
            CheckInternedSingleProperty(gb);
        }

        private void CheckInternedSingleProperty(SingleProperty sp)
        {
            var groups = sp.GetEntries()
                .Select(x => x.Value)
                .GroupBy(x => x);

            foreach (var g in groups)
            {
                var first = g.First();
                foreach (var x in g)
                {
                    Assert.Same(x, first);
                }
            }
        }
    }
}
