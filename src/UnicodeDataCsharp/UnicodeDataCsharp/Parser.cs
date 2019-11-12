using System;
using System.Text;

namespace UnicodeDataCsharp
{
    internal class Parser
    {
        private static readonly Encoding _utf8 = Encoding.UTF8;

        public static string GetString(ReadOnlyMemory<byte> utf8) => _utf8.GetString(utf8.Span);

        public static bool ParseYN(ReadOnlyMemory<byte> utf8)
        {
            if (utf8.Length == 0) throw new InvalidOperationException();
            return utf8.Span[0] == (byte)'Y';
        }
    }
}
