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
        /// Rune (Unicode Scalar、サロゲートペアの片割れみたいな不正な値を除外したやつ)になってない Code Point
        /// (要するに不正な値)を見かけたこともないので <see cref="Rune"/> を返す。
        /// </remarks>
        public static Rune ParseRune(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return new Rune(x);
            throw new InvalidOperationException();
        }

        /// <summary>
        /// <see cref="Rune"/> で、フィールドが空っぽのことがあるやつ。
        /// 空っぽだったら null を返す。
        /// </summary>
        public static Rune? ParseRuneOpt(ReadOnlyMemory<byte> utf8)
        {
            if (Utf8Parser.TryParse(utf8.Span, out uint x, out _, 'X')) return new Rune(x);
            else return null;
        }

        /// <summary>
        /// 0600..0605 みたいなフィールドを parse。
        /// 16進数単体か、16進数2個を .. でつないだものかのどちらか。
        /// </summary>
        public static RuneRange ParseRuneRange(ReadOnlyMemory<byte> utf8)
        {
            var s = utf8.Span;
            var i = 0;
            for (; i < s.Length; i++)
            {
                if (s[i] == '.') break;
            }

            if(i == s.Length)
            {
                var rune = ParseRune(utf8);
                return new RuneRange(rune, rune);
            }

            if(s[i + 1] != '.') throw new InvalidOperationException();

            var start = s[..i];
            var end = s[(i + 2)..];

            if (!Utf8Parser.TryParse(start, out uint x, out _, 'X')) throw new InvalidOperationException();
            if (!Utf8Parser.TryParse(end, out uint y, out _, 'X')) throw new InvalidOperationException();
            return new RuneRange(x, y + 1);
        }
    }
}
