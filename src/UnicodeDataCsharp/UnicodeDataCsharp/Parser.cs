using System;
using System.Buffers.Text;
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

        public static Rune ParseRune(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return new Rune(x);
            throw new InvalidOperationException();
        }

        public static Rune? ParseRuneOpt(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return new Rune(x);
            else return null;
        }
    }
}
