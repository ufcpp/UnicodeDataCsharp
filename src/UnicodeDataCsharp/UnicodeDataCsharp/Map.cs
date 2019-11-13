using System;
using System.Diagnostics;

namespace UnicodeDataCsharp
{
    /// <summary>
    /// キーとして <see cref="ReadOnlySpan{byte}"/> を受け付ける UTF-8 → UTF-16 (string)の辞書。
    /// </summary>
    /// <remarks>
    /// そんなに汎用性あるわけでもないけど、テスト用に public に。
    /// </remarks>
    public class Map
    {
        private static readonly Entry[] InitialEntries = new Entry[1];
        private int _count;
        private int _freeList = -1;
        private int[] _buckets;
        private Entry[] _entries;


        private struct Entry
        {
            public byte[] utf8;
            public string utf16;
            public int next;
        }

        /// <summary>
        /// 既定の容量で初期化。
        /// </summary>
        public Map()
        {
            _buckets = MapHelpers.SizeOneIntArray;
            _entries = InitialEntries;
        }

        /// <summary>
        /// 容量を指定して初期化。
        /// </summary>
        /// <param name="capacity">初期容量。</param>
        public Map(int capacity)
        {
            if (capacity < 0)
                MapHelpers.ThrowCapacityArgumentOutOfRangeException();
            if (capacity < 2)
                capacity = 2; // 1 would indicate the dummy array
            capacity = MapHelpers.PowerOf2(capacity);
            _buckets = new int[capacity];
            _entries = new Entry[capacity];
        }

        /// <summary>
        /// 辞書内にある要素数。
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 全削除。
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _freeList = -1;
            _buckets = MapHelpers.SizeOneIntArray;
            _entries = InitialEntries;
        }

        /// <summary>
        /// 指定したキーを削除。
        /// </summary>
        public bool Remove(ReadOnlySpan<byte> key)
        {
            Entry[] entries = _entries;
            int bucketIndex = GetHashCode(key) & (_buckets.Length - 1);
            int entryIndex = _buckets[bucketIndex] - 1;

            int lastIndex = -1;
            int collisionCount = 0;
            while (entryIndex != -1)
            {
                var candidate = entries[entryIndex];
                if (key.SequenceEqual(candidate.utf8))
                {
                    if (lastIndex != -1)
                    {   // Fixup preceding element in chain to point to next (if any)
                        entries[lastIndex].next = candidate.next;
                    }
                    else
                    {   // Fixup bucket to new head (if any)
                        _buckets[bucketIndex] = candidate.next + 1;
                    }

                    entries[entryIndex] = default;

                    entries[entryIndex].next = -3 - _freeList; // New head of free list
                    _freeList = entryIndex;

                    _count--;
                    return true;
                }
                lastIndex = entryIndex;
                entryIndex = candidate.next;

                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    MapHelpers.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                }
                collisionCount++;
            }

            return false;
        }

        public string GetString(ReadOnlySpan<byte> utf8)
        {
            ref var value = ref GetOrAddValueRef(utf8);
            if (value == null) value = Parser.GetString(utf8);
            return value;
        }

        private static int GetHashCode(ReadOnlySpan<byte> utf8)
        {
            var h = default(HashCode);
            foreach (var x in utf8) h.Add(x);
            return h.ToHashCode();
        }

        /// <summary>
        /// 指定したキーがあればその値を、なければ既定値で新規追加して返す。
        /// </summary>
        /// <remarks>
        /// ref を返してるので、メソッド内でスレッド安全性の確保が絶対できないので、
        /// 同時実行が必要な時は注意を。
        /// </remarks>
        private ref string GetOrAddValueRef(ReadOnlySpan<byte> key)
        {
            Entry[] entries = _entries;
            int collisionCount = 0;
            int bucketIndex = GetHashCode(key) & (_buckets.Length - 1);
            for (int i = _buckets[bucketIndex] - 1;
                    (uint)i < (uint)entries.Length; i = entries[i].next)
            {
                if (key.SequenceEqual(entries[i].utf8))
                    return ref entries[i].utf16;
                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    MapHelpers.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                }
                collisionCount++;
            }

            return ref AddKey(key, bucketIndex);
        }

        private ref string AddKey(ReadOnlySpan<byte> key, int bucketIndex)
        {
            Entry[] entries = _entries;
            int entryIndex;
            if (_freeList != -1)
            {
                entryIndex = _freeList;
                _freeList = -3 - entries[_freeList].next;
            }
            else
            {
                if (_count == entries.Length || entries.Length == 1)
                {
                    entries = Resize();
                    bucketIndex = GetHashCode(key) & (_buckets.Length - 1);
                    // entry indexes were not changed by Resize
                }
                entryIndex = _count;
            }

            entries[entryIndex].utf8 = key.ToArray();
            entries[entryIndex].next = _buckets[bucketIndex] - 1;
            _buckets[bucketIndex] = entryIndex + 1;
            _count++;
            return ref entries[entryIndex].utf16;
        }

        private Entry[] Resize()
        {
            Debug.Assert(_entries.Length == _count || _entries.Length == 1); // We only copy _count, so if it's longer we will miss some
            int count = _count;
            int newSize = _entries.Length * 2;
            if ((uint)newSize > (uint)int.MaxValue) // uint cast handles overflow
                MapHelpers.ThrowCapacityOverflow();

            var entries = new Entry[newSize];

            Array.Copy(_entries, 0, entries, 0, count);

            var newBuckets = new int[entries.Length];
            while (count-- > 0)
            {
                int bucketIndex = GetHashCode(entries[count].utf8) & (newBuckets.Length - 1);
                entries[count].next = newBuckets[bucketIndex] - 1;
                newBuckets[bucketIndex] = count + 1;
            }

            _buckets = newBuckets;
            _entries = entries;

            return entries;
        }
    }

    internal class MapHelpers
    {
        internal static readonly int[] SizeOneIntArray = new int[1];

        internal static int PowerOf2(int v)
        {
            if ((v & (v - 1)) == 0) return v;
            int i = 2;
            while (i < v) i <<= 1;
            return i;
        }

        internal static void ThrowCapacityArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("capacity");
        internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported() => throw new InvalidOperationException("concurrent operations not supported");
        internal static void ThrowKeyArgumentNullException() => throw new ArgumentNullException("key");
        internal static void ThrowCapacityOverflow() => throw new InvalidOperationException("capacity overflow");
    }
}
