using Cistern.Linq.ChainLinq.Optimizations;
using System;
using System.Collections;
using System.Collections.Immutable;

namespace Cistern.Linq.Immutable
{
    struct ImmutableArrayEnumerator<T>
        : System.Collections.Generic.IEnumerator<T>
    {
        private ImmutableArray<T>.Enumerator e;

        public ImmutableArrayEnumerator(ImmutableArray<T> source) => e = source.GetEnumerator();
        public void Dispose() { }

        public T Current => e.Current;

        public bool MoveNext() => e.MoveNext();

        public void Reset() => throw new NotImplementedException();
        object IEnumerator.Current => throw new NotImplementedException();
    }

    struct ImmutableArrayEnumerable<T>
        : ITypedEnumerable<T, ImmutableArrayEnumerator<T>>
    {
        private ImmutableArray<T> source;
        public ImmutableArrayEnumerable(ImmutableArray<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;

        public int? TryLength => source.Length;

        public ImmutableArrayEnumerator<T> GetEnumerator() => new ImmutableArrayEnumerator<T>(source);
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = source.AsSpan();
            return true;
        }
        public bool TryLast(out T result)
        {
            if (source.Length == 0)
            {
                result = default;
                return false;
            }
            result = source[source.Length - 1];
            return true;
        }
    }

    struct ImmutableHashSetEnumerable<T>
        : ITypedEnumerable<T, ImmutableHashSet<T>.Enumerator>
    {
        private ImmutableHashSet<T> source;
        public ImmutableHashSetEnumerable(ImmutableHashSet<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;

        public int? TryLength => source.Count; // is this O(1)??

        public ImmutableHashSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
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

    struct ImmutableListEnumerable<T>
        : ITypedEnumerable<T, ImmutableList<T>.Enumerator>
    {
        private ImmutableList<T> source;
        public ImmutableListEnumerable(ImmutableList<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;
        public int? TryLength => source.Count; // is this O(1)??

        public ImmutableList<T>.Enumerator GetEnumerator() => source.GetEnumerator();
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

    struct ImmutableQueueEnumerator<T>
        : System.Collections.Generic.IEnumerator<T>
    {
        private ImmutableQueue<T>.Enumerator e;

        public ImmutableQueueEnumerator(ImmutableQueue<T> source) => e = source.GetEnumerator();
        public void Dispose() { }

        public T Current => e.Current;

        public bool MoveNext() => e.MoveNext();

        public void Reset() => throw new NotImplementedException();
        object IEnumerator.Current => throw new NotImplementedException();
    }

    struct ImmutableQueueEnumerable<T>
        : ITypedEnumerable<T, ImmutableQueueEnumerator<T>>
    {
        private ImmutableQueue<T> source;
        public ImmutableQueueEnumerable(ImmutableQueue<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;
        public int? TryLength => null;

        public ImmutableQueueEnumerator<T> GetEnumerator() => new ImmutableQueueEnumerator<T>(source);
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TryLast(out T result)
        {
            // TODO: Check Peek? or is that the wrong end?
            result = default;
            return false;
        }
    }

    struct ImmutableSortedSetEnumerable<T>
        : ITypedEnumerable<T, ImmutableSortedSet<T>.Enumerator>
    {
        private ImmutableSortedSet<T> source;
        public ImmutableSortedSetEnumerable(ImmutableSortedSet<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;
        public int? TryLength => source.Count; // is this O(1)??

        public ImmutableSortedSet<T>.Enumerator GetEnumerator() => source.GetEnumerator();
        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TryLast(out T result)
        {
            // TODO: Sorted, so should be able to pull off last, need to check API
            result = default;
            return false;
        }
    }

    struct ImmutableStackEnumerator<T>
        : System.Collections.Generic.IEnumerator<T>
    {
        private ImmutableStack<T>.Enumerator e;

        public ImmutableStackEnumerator(ImmutableStack<T> source) => e = source.GetEnumerator();
        public void Dispose() { }

        public T Current => e.Current;

        public bool MoveNext() => e.MoveNext();

        public void Reset() => throw new NotImplementedException();
        object IEnumerator.Current => throw new NotImplementedException();
    }

    struct ImmutableStackEnumerable<T>
        : ITypedEnumerable<T, ImmutableStackEnumerator<T>>
    {
        private ImmutableStack<T> source;
        public ImmutableStackEnumerable(ImmutableStack<T> source) => this.source = source;

        public System.Collections.Generic.IEnumerable<T> Source => source;
        public int? TryLength => null;

        public ImmutableStackEnumerator<T> GetEnumerator() => new ImmutableStackEnumerator<T>(source);
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
