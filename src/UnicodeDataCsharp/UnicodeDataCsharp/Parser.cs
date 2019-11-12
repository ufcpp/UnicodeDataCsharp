using System;
using System.Text;

namespace UnicodeDataCsharp
{
    internal class Parser
    {
        private static readonly Encoding _utf8 = Encoding.UTF8;

        public static string GetString(ReadOnlyMemory<byte> utf8) => _utf8.GetString(utf8.Span);
    }
}
