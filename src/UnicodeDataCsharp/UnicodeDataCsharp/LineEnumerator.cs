using System;
using System.Collections;
using System.Collections.Generic;

namespace UnicodeDataCsharp
{
    /// <summary>
    /// 各行のデータを文字列のまま読むんじゃなくて、C# の型にデシリアライズして使いたいとき用のインターフェイス。
    /// default(T).New(line) みたいに使う想定。
    /// </summary>
    /// <typeparam name="Line">各行を表す型。</typeparam>
    public interface ILineFactory<Line>
    {
        Line New(ReadOnlyMemory<byte> line);
    }

    public struct LineEnumerable<Line, LineFactory> : IEnumerable<Line>
        where LineFactory : struct, ILineFactory<Line>
    {
        private readonly byte[] _utf8;
        public LineEnumerable(byte[] utf8) => _utf8 = utf8;
        public LineEnumerator<Line, LineFactory> GetEnumerator() => new LineEnumerator<Line, LineFactory>(_utf8);
        IEnumerator<Line> IEnumerable<Line>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// 行分割。
    /// </summary>
    /// <remarks>
    /// '\r'? 知らない子ですね。
    ///
    /// 今のところ UnicodeData.txt でしか使ってないからいいけど、他のデータも参照するなら、
    /// - # から後ろはコメントとして無視
    /// - 空行は読み飛ばす
    /// みたいな処理も必用。
    /// </remarks>
    public struct LineEnumerator<Line, LineFactory> : IEnumerator<Line>
        where LineFactory : struct, ILineFactory<Line>
    {
        private readonly byte[] _utf8;
        private int _start;
        private int _end;
        public LineEnumerator(byte[] utf8) => (_utf8, _start, _end) = (utf8, 0, -1);

        public Line Current => default(LineFactory).New(_utf8.AsMemory().Slice(_start, _end - _start));

        public bool MoveNext()
        {
            while (true)
            {
                if (!MoveNextInternal()) return false;

                // 空行・コメントのみの行を読み飛ばし。
                var startChar = _utf8[_start];
                if (startChar != '\n' && startChar != '#') return true;
            }
        }

        // 素直に \n が見つかるまで
        private bool MoveNextInternal()
        {
            var s = _utf8;

            if (_end == s.Length) return false;

            var start = _end + 1;

            if (start == s.Length)
            {
                _end = s.Length;
                return false;
            }

            int i;
            for (i = start; i < s.Length; i++)
            {
                if (s[i] == '\n') break;
            }

            _start = start;
            _end = i;
            return true;
        }

        object System.Collections.IEnumerator.Current => Current!;
        public void Dispose() { }
        public void Reset() => throw new NotImplementedException();
    }
}
