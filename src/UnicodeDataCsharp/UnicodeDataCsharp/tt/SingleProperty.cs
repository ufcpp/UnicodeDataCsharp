using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnicodeDataCsharp
{
    using static Parser;

    public partial struct SingleProperty : IEnumerable<SingleProperty.Line>
    {
        private readonly byte[] _data;
        public SingleProperty(byte[] data) => _data = data;

        public LineEnumerator<Line, LineFactory> GetEnumerator() => new LineEnumerator<Line, LineFactory>(_data);
        IEnumerator<Line> IEnumerable<Line>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct LineFactory : ILineFactory<Line>
        {
            public Line New(ReadOnlyMemory<byte> line) => new Line(line);
        }

        // この型の時点ではアロケーションなしで、ReadOnlyMemory{T} を使ってスプリットしてる。
        public readonly partial struct Line
        {
            private readonly ReadOnlyMemory<byte> _rawData;
            public Line(ReadOnlyMemory<byte> rawData) => _rawData = rawData;

            public override string ToString() => GetString(_rawData);

            public ReadOnlyMemory<byte> Range => GetValue(0);
            public ReadOnlyMemory<byte> Value => GetValue(1);

            private ReadOnlyMemory<byte> GetValue(int column) => LineSplitter.GetValue(_rawData, column);

            /// <summary>
            /// <see cref="Entry"/>
            /// </summary>
            public Entry GetEntry() => new Entry(this);
        }

        // Line の各フィールドを parse。
        public readonly partial struct Entry
        {
            public RuneRange Range { get; }
            public string Value { get; }

            public Entry(Line line)
            {
                Range = ParseRuneRange(line.Range);
                Value = GetInternString(line.Value);
            }

            public override string ToString() => (Range, Value).ToString();
        }

        public IEnumerable<Entry> GetEntries()
        {
            foreach (var line in this) yield return line.GetEntry();
        }
    }
}
