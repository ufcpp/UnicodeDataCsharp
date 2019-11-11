using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnicodeDataCsharp
{
    public struct UnicodeData : IEnumerable<UnicodeData.Line>
    {
        private readonly byte[] _data;
        public UnicodeData(byte[] data) => _data = data;

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

            public override string ToString() => Encoding.UTF8.GetString(_rawData.Span);

            public ReadOnlyMemory<byte> CodePoint => GetValue(0);
            public ReadOnlyMemory<byte> Name => GetValue(1);
            public ReadOnlyMemory<byte> GeneralCategory => GetValue(2);
            public ReadOnlyMemory<byte> CombiningCategory => GetValue(3);
            public ReadOnlyMemory<byte> BidirectionalCategory => GetValue(4);
            public ReadOnlyMemory<byte> DecompositionMapping => GetValue(5);
            public ReadOnlyMemory<byte> DecimalDigit => GetValue(6);
            public ReadOnlyMemory<byte> Digit => GetValue(7);
            public ReadOnlyMemory<byte> Numeric => GetValue(8);
            public ReadOnlyMemory<byte> Mirrored => GetValue(9);
            public ReadOnlyMemory<byte> Unicode1Name => GetValue(10);
            public ReadOnlyMemory<byte> Comment => GetValue(11);
            public ReadOnlyMemory<byte> UpperCasMapping => GetValue(12);
            public ReadOnlyMemory<byte> LowerCasMapping => GetValue(13);
            public ReadOnlyMemory<byte> TitleCasMapping => GetValue(14);

            private ReadOnlyMemory<byte> GetValue(int column) => LineSplitter.GetValue(_rawData, column);

            /// <summary>
            /// <see cref="Entry"/>
            /// </summary>
            public Entry GetEntry() => new Entry(this);
        }

        // Line の各フィールドを parse。
        public class Entry
        {
            public string CodePoint { get; }
            public string Name { get; }
            public string GeneralCategory { get; }
            public string CombiningCategory { get; }
            public string BidirectionalCategory { get; }
            public string DecompositionMapping { get; }
            public string DecimalDigit { get; }
            public string Digit { get; }
            public string Numeric { get; }
            public string Mirrored { get; }
            public string Unicode1Name { get; }
            public string Comment { get; }
            public string UpperCasMapping { get; }
            public string LowerCasMapping { get; }
            public string TitleCasMapping { get; }

            public Entry(Line line)
            {
                CodePoint = Encoding.UTF8.GetString(line.CodePoint.Span);
                Name = Encoding.UTF8.GetString(line.Name.Span);
                GeneralCategory = Encoding.UTF8.GetString(line.GeneralCategory.Span);
                CombiningCategory = Encoding.UTF8.GetString(line.CombiningCategory.Span);
                BidirectionalCategory = Encoding.UTF8.GetString(line.BidirectionalCategory.Span);
                DecompositionMapping = Encoding.UTF8.GetString(line.DecompositionMapping.Span);
                DecimalDigit = Encoding.UTF8.GetString(line.DecimalDigit.Span);
                Digit = Encoding.UTF8.GetString(line.Digit.Span);
                Numeric = Encoding.UTF8.GetString(line.Numeric.Span);
                Mirrored = Encoding.UTF8.GetString(line.Mirrored.Span);
                Unicode1Name = Encoding.UTF8.GetString(line.Unicode1Name.Span);
                Comment = Encoding.UTF8.GetString(line.Comment.Span);
                UpperCasMapping = Encoding.UTF8.GetString(line.UpperCasMapping.Span);
                LowerCasMapping = Encoding.UTF8.GetString(line.LowerCasMapping.Span);
                TitleCasMapping = Encoding.UTF8.GetString(line.TitleCasMapping.Span);
            }

            public override string ToString() => (CodePoint, Name, GeneralCategory, CombiningCategory, BidirectionalCategory, DecompositionMapping, DecimalDigit, Digit, Numeric, Mirrored, Unicode1Name, Comment, UpperCasMapping, LowerCasMapping, TitleCasMapping).ToString();
        }

        public IEnumerable<Entry> GetEntries()
        {
            foreach (var line in this) yield return line.GetEntry();
        }
    }
}
