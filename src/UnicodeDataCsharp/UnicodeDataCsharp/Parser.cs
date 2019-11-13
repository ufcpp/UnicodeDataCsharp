using System;
using System.Buffers.Text;
using System.Text;

namespace UnicodeDataCsharp
{
    internal class Parser
    {
        private static readonly Encoding _utf8 = Encoding.UTF8;

        public static string GetString(ReadOnlyMemory<byte> utf8) => GetString(utf8.Span);
        public static string GetString(ReadOnlySpan<byte> utf8) => _utf8.GetString(utf8);

        private static readonly Map _internString = new Map();

        /// <summary>
        /// 文字列なんだけど、限られた種類しか来ないことがわかっているものに使う。
        /// 同じ UTF-8 <see cref="ReadOnlySpan{byte}"/> からは常に同一インスタンスの string を返す。
        /// </summary>
        public static string GetInternString(ReadOnlyMemory<byte> utf8) => _internString.GetString(utf8.Span);

        /// <summary>
        /// Y か N か(それぞれ Yes/No の意味。true/false として解釈)のフィールドを parse。
        /// </summary>
        public static bool ParseYN(ReadOnlyMemory<byte> utf8)
        {
            if (utf8.Length == 0) throw new InvalidOperationException();
            return utf8.Span[0] == (byte)'Y';
        }

        /// <summary>
        /// 16進数で Code Point が掛かれてるフィールドを parse。
        /// </summary>
        /// <remarks>
        /// UCD 中、Code Point は16進数で書かれてる。
        /// (知っている範囲では全部１６進数。)
        /// </remarks>
        public static uint ParseCodePoint(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return x;
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Code Point で、フィールドが空っぽのことがあるやつ。
        /// 空っぽだったら null を返す。
        /// </summary>
        public static uint? ParseCodePointOpt(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return x;
            else return null;
        }

        /// <summary>
        /// 0600..0605 みたいなフィールドを parse。
        /// 16進数単体か、16進数2個を .. でつないだものかのどちらか。
        /// </summary>
        public static CodePointRange ParseCodePointRange(ReadOnlyMemory<byte> utf8)
        {
            var s = utf8.Span;
            var i = 0;
            for (; i < s.Length; i++)
            {
                if (s[i] == '.') break;
            }

            if(i == s.Length)
            {
                var cp = ParseCodePoint(utf8);
                return new CodePointRange(cp);
            }

            if(s[i + 1] != '.') throw new InvalidOperationException();

            var start = s[..i];
            var end = s[(i + 2)..];

            if (!Utf8Parser.TryParse(start, out uint x, out _, 'X')) throw new InvalidOperationException();
            if (!Utf8Parser.TryParse(end, out uint y, out _, 'X')) throw new InvalidOperationException();
            return new CodePointRange(x, y + 1);
        }
    }
}
