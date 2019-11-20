using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnicodeDataCsharp;
using Xunit;

namespace UnicodeDataCsharpTest
{
    public class RangeGetValueTest
    {

        [Fact]
        public async Task GetValueWithGraphemeBreakProperty()
        {
            var gb = await UcdTest.Ucd.GetGraphemeBreakPropertyEntries();
            GetValue(gb);
        }

        private void GetValue(SingleProperty.Entry[] ranges)
        {
            Assert.Null(ranges.GetValue(0));
            Assert.Null(ranges.GetValue(0x500));

            Assert.Null(ranges.GetValue(0x5FF));
            Assert.Equal("Prepend", ranges.GetValue(0x600));
            Assert.Equal("Prepend", ranges.GetValue(0x603));
            Assert.Equal("Prepend", ranges.GetValue(0x605));
            Assert.Null(ranges.GetValue(0x606));

            Assert.Null(ranges.GetValue(0x60C));
            Assert.Equal("Prepend", ranges.GetValue(0x60D));
            Assert.Null(ranges.GetValue(0x60E));
        }
    }
}
