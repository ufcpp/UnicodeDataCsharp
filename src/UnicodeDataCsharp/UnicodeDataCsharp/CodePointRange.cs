using System.Text;

namespace UnicodeDataCsharp
{
    /// <summary>
    /// 連続した Code Point の範囲。
    /// </summary>
    /// <remarks>
    /// 例えば GraphemeBreakProperty.txt に以下のようなデータあり。
    ///
    /// 0600..0605    ; Prepend
    /// 06DD          ; Prepend
    ///
    /// これの第1フィールドにあたる型。
    /// </remarks>
    public readonly struct CodePointRange
    {
        /// <summary>
        /// 範囲の開始コード。inclusive。
        /// </summary>
        public readonly uint Start;

        /// <summary>
        /// 範囲の末尾コード。exclusive。
        /// <see cref="Start"/> と同じ値(範囲に1つだけ値がある)の場合あり。
        /// </summary>
        /// <remarks>
        /// <see cref="ToString"/> では -1 して inclusive に戻して表示してる。
        /// </remarks>
        public readonly uint End;

        public CodePointRange(uint codePoint) => (Start, End) = (codePoint, codePoint);
        public CodePointRange(uint start, uint end) => (Start, End) = (start, end);

        public override string ToString()
            => Start == End
            ? $"{Start:X}"
            : $"{Start:X}..{End - 1:X}";
    }
}
