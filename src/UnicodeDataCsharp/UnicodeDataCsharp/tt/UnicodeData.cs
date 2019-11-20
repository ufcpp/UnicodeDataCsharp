using System;
using System.Collections;
using System.Collections.Generic;

namespace UnicodeDataCsharp
{
    using static Parser;

    public partial struct UnicodeData : IEnumerable<UnicodeData.Line>
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

        // Uses ReadOnlyMemory<T> for avoiding allocations
        public readonly partial struct Line
        {
            private readonly ReadOnlyMemory<byte> _rawData;
            public Line(ReadOnlyMemory<byte> rawData) => _rawData = rawData;

            public override string ToString() => GetString(_rawData);

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

        // Parses each fields of Line.
        public partial class Entry
        {
            public uint CodePoint { get; }
            public string Name { get; }
            public string GeneralCategory { get; }
            public string CombiningCategory { get; }
            public string BidirectionalCategory { get; }
            public string DecompositionMapping { get; }
            public string DecimalDigit { get; }
            public string Digit { get; }
            public string Numeric { get; }
            public bool Mirrored { get; }
            public string Unicode1Name { get; }
            public string Comment { get; }
            public uint? UpperCasMapping { get; }
            public uint? LowerCasMapping { get; }
            public uint? TitleCasMapping { get; }

            public Entry(Line line)
            {
                CodePoint = ParseCodePoint(line.CodePoint);
                Name = GetString(line.Name);
                GeneralCategory = GetInternString(line.GeneralCategory);
                CombiningCategory = GetString(line.CombiningCategory);
                BidirectionalCategory = GetInternString(line.BidirectionalCategory);
                DecompositionMapping = GetString(line.DecompositionMapping);
                DecimalDigit = GetString(line.DecimalDigit);
                Digit = GetString(line.Digit);
                Numeric = GetString(line.Numeric);
                Mirrored = ParseYN(line.Mirrored);
                Unicode1Name = GetString(line.Unicode1Name);
                Comment = GetString(line.Comment);
                UpperCasMapping = ParseCodePointOpt(line.UpperCasMapping);
                LowerCasMapping = ParseCodePointOpt(line.LowerCasMapping);
                TitleCasMapping = ParseCodePointOpt(line.TitleCasMapping);
            }

            public override string ToString() => (CodePoint, Name, GeneralCategory, CombiningCategory, BidirectionalCategory, DecompositionMapping, DecimalDigit, Digit, Numeric, Mirrored, Unicode1Name, Comment, UpperCasMapping, LowerCasMapping, TitleCasMapping).ToString();
        }

        public IEnumerable<Entry> GetEntries()
        {
            foreach (var line in this) yield return line.GetEntry();
        }
    }
}

