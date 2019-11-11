using System;

namespace UnicodeDataCsharp
{
    public class LineSplitter
    {
        /// <summary>
        /// UCD のデータは大体 ; 区切りなので、それ用の処理。
        /// </summary>
        /// <param name="rawData">行。</param>
        /// <param name="column">何コラム目を取りたいか。要は、この数だけ ; を読み飛ばす。</param>
        /// <returns>コラム。</returns>
        /// <remarks>
        /// 常に「先頭から指定個数の ; を読み飛ばす」って処理してるので、
        /// フィールドを前から順に、全部読むなら1回1回このメソッドを呼ぶのはもったいないんだけど。
        /// そうまでして効率求めてもしょうがなさそうなのであきらめてる。
        ///
        /// <see cref="LineEnumerator{Line, LineFactory}"/>。
        /// # から後ろはコメントだから無視って処理は、やるとしたらこっちでやる方が楽かも。
        /// </remarks>
        public static ReadOnlyMemory<byte> GetValue(ReadOnlyMemory<byte> rawData, int column)
        {
            var s = rawData.Span;
            var start = 0;
            for (int c = 0; c < column; c++)
            {
                for (; start < s.Length; start++)
                {
                    if (s[start] == ';') break;
                }
                start++;
            }

            int end;
            for (end = start; end < s.Length; end++)
            {
                if (s[end] == ';' || s[end] == '#') break;
            }

            return rawData.Slice(start, end - start);
        }
    }
}
