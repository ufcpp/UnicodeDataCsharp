using System.Runtime.CompilerServices;

namespace UnicodeDataCsharp
{
    public static class SinglePropertyExtensions
    {
        /// <summary>
        /// <paramref name="ranges"/> から二分探索で <paramref name="codePoint"/> の <see cref="SingleProperty.Entry.Value"/> を検索。
        /// </summary>
        /// <param name="ranges">ソート済みの値一覧。</param>
        /// <param name="codePoint">検索したい文字のコードポイント。</param>
        /// <returns>見つかればその値、見つからなければ null。</returns>
        /// <remarks>
        /// 二分探索なので当然 <paramref name="codePoint"/> がソート済みでないといけない。
        /// 同要素の <see cref="CodePointRange.Start"/> は <see cref="CodePointRange.End"/> よりも常に同じか小さく、
        /// 1要素前の <see cref="CodePointRange.End"/> が後の <see cref="CodePointRange.Start"/> よりも常に小さい必要あり。
        ///
        /// 普通に UCD から読んだデータ、値ごとにまとめられてたりして Range 側でのソートされてないので事前にソート必要。
        /// </remarks>
        public static string? GetValue(this SingleProperty.Entry[] ranges, uint codePoint)
        {
            // ÷2 を >>1 で置き換えるために uint
            uint lower = 0;
            uint upper = (uint)(ranges.Length - 1);
            ref var p = ref ranges[0];

            while (lower <= unchecked((int)upper))
            {
                uint middle = (upper + lower) >> 1;
                ref var r = ref Unsafe.Add(ref p, (int)middle); // ranges[middle] の範囲チェック避け
                if (codePoint < r.Range.Start) upper = middle - 1;
                else
                {
                    var end = Unsafe.Add(ref r, 1).Range.Start;
                    if (codePoint > end) lower = middle + 1;
                    else return r.Value;
                }
            }

            // テーブル的に来ないとはおもうんだけど、
            return null;
        }
    }
}
