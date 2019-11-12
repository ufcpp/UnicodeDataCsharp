using System.Text;

namespace UnicodeDataCsharp
{
    /// <summary>
    /// 連続した <see cref="Rune"/> の範囲。
    /// </summary>
    /// <remarks>
    /// 例えば GraphemeBreakProperty.txt に以下のようなデータあり。
    ///
    /// 0600..0605    ; Prepend
    /// 06DD          ; Prepend
    ///
    /// これの第1フィールドにあたる型。
    /// </remarks>
    public readonly struct RuneRange
    {
        /// <summary>
        /// 範囲の開始コード。inclusive。
        /// </summary>
        public readonly Rune Start;

        /// <summary>
        /// 範囲の末尾コード。exclusive。
        /// <see cref="Start"/> と同じ値(範囲に1つだけ値がある)の場合あり。
        /// </summary>
        /// <remarks>
        /// <see cref="ToString"/> では -1 して inclusive に戻して表示してる。
        /// </remarks>
        public readonly Rune End;

        public RuneRange(Rune rune) => (Start, End) = (rune, rune);
        public RuneRange(Rune start, Rune end) => (Start, End) = (start, end);
        public RuneRange(uint start, uint end) => (Start, End) = (new Rune(start), new Rune(end));

        public override string ToString()
            => Start == End
            ? $"{Start.Value:X}"
            : $"{Start.Value:X}..{End.Value - 1:X}";
    }
}
