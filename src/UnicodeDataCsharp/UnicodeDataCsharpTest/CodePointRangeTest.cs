using System;
using System.Collections.Generic;
using System.Text;
using UnicodeDataCsharp;
using Xunit;

namespace UnicodeDataCsharpTest
{
    public class CodePointRangeTest
    {
        private static readonly Encoding ascii = Encoding.ASCII;

        [Fact]
        public void Parse()
        {
            var data = new[]
            {
                (ascii.GetBytes("1"), 1, 1),
                (ascii.GetBytes("1..2"), 1, 2),
                (ascii.GetBytes("5..a0"), 5, 0xA0),
                (ascii.GetBytes("1AF..A00"), 0x1AF, 0xA00),
                (ascii.GetBytes("2f97..5DcB"), 0x2F97, 0x5DCB),
                (ascii.GetBytes("1A000..1B000"), 0x1A000, 0x1B000),
            };

            foreach (var (m, s, e) in data)
            {
                var r = Parser.ParseCodePointRange(m);
                Assert.Equal((uint)s, r.Start);
                Assert.Equal((uint)e, r.End);
            }
        }
    }
}
