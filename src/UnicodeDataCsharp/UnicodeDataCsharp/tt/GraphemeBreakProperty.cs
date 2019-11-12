using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnicodeDataCsharp
{
    using static Parser;

    public struct GraphemeBreakProperty : IEnumerable<GraphemeBreakProperty.Line>
    {
        private readonly byte[] _data;
        public GraphemeBreakProperty(byte[] data) => _data = data;

        public LineEnumerator<Line, LineFactory> GetEnumerator() => new LineEnumerator<Line, LineFactory>(_data);
        IEnumerator<Line> IEnumerable<Line>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct LineFactory : ILineFactory<Line>
        {
            public Line New(ReadOnlyMemory<byte> line) => new Line(line);
        }

        // この型の時点ではアロケーションなしで、ReadOnlyMemory{T} を使ってスプリットしてる。
        public readonly struct Line
        {
            private readonly ReadOnlyMemory<byte> _rawData;
            public Line(ReadOnlyMemory<byte> rawData) => _rawData = rawData;

            public override string ToString() => GetString(_rawData);

            public ReadOnlyMemory<byte> CodePoint => GetValue(0);
            public ReadOnlyMemory<byte> Value => GetValue(1);

            private ReadOnlyMemory<byte> GetValue(int column) => LineSplitter.GetValue(_rawData, column);

            /// <summary>
            /// <see cref="Entry"/>
            /// </summary>
            public Entry GetEntry() => new Entry(this);
        }

        // Line の各フィールドを parse。
        public class Entry
        {
            public RuneRange CodePoint { get; }
            public string Value { get; }

            public Entry(Line line)
            {
                CodePoint = ParseRuneRange(line.CodePoint);
                Value = GetInternString(line.Value);
            }

            public override string ToString() => (CodePoint, Value).ToString();
        }

        public IEnumerable<Entry> GetEntries()
        {
            foreach (var line in this) yield return line.GetEntry();
        }
    }
}
