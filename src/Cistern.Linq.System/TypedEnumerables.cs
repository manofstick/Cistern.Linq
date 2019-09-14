using Cistern.Linq.ChainLinq.Optimizations;
using System;
using System.Collections.Generic;

namespace Cistern.Linq.Immutable
{
    struct LinkedListEnumerable<T>
        : ITypedEnumerable<T, LinkedListEnumerable<T>, LinkedList<T>.Enumerator>
    {
        private LinkedList<T> source;
        public LinkedListEnumerable(LinkedList<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public int? TryLength => source.Count;

        public LinkedList<T>.Enumerator GetEnumerator() => source.GetEnumerator();

        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TrySkip(int count, out LinkedListEnumerable<T> skipped)
        {
            skipped = default;
            return false;
        }
    }

    struct HashSetEnumerable<T>
        : ITypedEnumerable<T, HashSetEnumerable<T>, HashSet<T>.Enumerator>
    {
        private HashSet<T> source;
        public HashSetEnumerable(HashSet<T> source) => this.source = source;

        public IEnumerable<T> Source => source;
        public int? TryLength => source.Count;

        public HashSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TrySkip(int count, out HashSetEnumerable<T> skipped)
        {
            skipped = default;
            return false;
        }
    }

    struct StackEnumerable<T>
        : ITypedEnumerable<T, StackEnumerable<T>, Stack<T>.Enumerator>
    {
        private Stack<T> source;
        public StackEnumerable(Stack<T> source) => this.source = source;

        public IEnumerable<T> Source => source;
        public int? TryLength => source.Count;

        public Stack<T>.Enumerator GetEnumerator() => source.GetEnumerator();
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TrySkip(int count, out StackEnumerable<T> skipped)
        {
            skipped = default;
            return false;
        }
    }

    struct QueueEnumerable<T>
        : ITypedEnumerable<T, QueueEnumerable<T>, Queue<T>.Enumerator>
    {
        private Queue<T> source;
        public QueueEnumerable(Queue<T> source) => this.source = source;

        public IEnumerable<T> Source => source;
        public int? TryLength => source.Count;

        public Queue<T>.Enumerator GetEnumerator() => source.GetEnumerator();
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TrySkip(int count, out QueueEnumerable<T> skipped)
        {
            skipped = default;
            return false;
        }
    }

    struct SortedSetEnumerable<T>
        : ITypedEnumerable<T, SortedSetEnumerable<T>, SortedSet<T>.Enumerator>
    {
        private SortedSet<T> source;
        public SortedSetEnumerable(SortedSet<T> source) => this.source = source;

        public IEnumerable<T> Source => source;
        public int? TryLength => source.Count;

        public SortedSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TrySkip(int count, out SortedSetEnumerable<T> skipped)
        {
            skipped = default;
            return false;
        }
    }
}
