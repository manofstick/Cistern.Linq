using Cistern.Linq.ChainLinq.Optimizations;
using System;
using System.Collections.Generic;

namespace Cistern.Linq.Immutable
{
    struct LinkedListEnumerable<T>
        : ITypedEnumerable<T, LinkedList<T>.Enumerator>
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

        public bool TryLast(out T result)
        {
            var last = source.Last;
            if (last == null)
            {
                result = default;
                return false;
            }
            result = last.Value;
            return true;
        }

    }

    struct HashSetEnumerable<T>
        : ITypedEnumerable<T, HashSet<T>.Enumerator>
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

        public bool TryLast(out T result)
        {
            result = default;
            return false;
        }
    }

    struct StackEnumerable<T>
        : ITypedEnumerable<T, Stack<T>.Enumerator>
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

        public bool TryLast(out T result)
        {
            result = default;
            return false;
        }
    }

    struct QueueEnumerable<T>
        : ITypedEnumerable<T, Queue<T>.Enumerator>
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

        public bool TryLast(out T result)
        {
            result = default;
            return false;
        }
    }

    struct SortedSetEnumerable<T>
        : ITypedEnumerable<T, SortedSet<T>.Enumerator>
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

        public bool TryLast(out T result)
        {
            result = default;
            return false;
        }
    }
}
