using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ITypedEnumerable<T, Enumerator>
        where Enumerator : IEnumerator<T>
    {
        IEnumerable<T> Source { get; }
        int? TryLength { get; }
        bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan);
        Enumerator GetEnumerator();
        bool TryLast(out T result);
    }

    struct IEnumerableEnumerable<T>
        : ITypedEnumerable<T, IEnumerator<T>>
    {
        public IEnumerableEnumerable(IEnumerable<T> source) => Source = source;

        public IEnumerable<T> Source { get; }

        public int? TryLength => Source is ICollection<T> ct ? (int?)ct.Count : Source is ICollection c ? (int?)c.Count : null;

        public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();

        public ReadOnlySpan<T> TryGetSourceAsSpan() => null;

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

    struct ListEnumerable<T>
        : ITypedEnumerable<T, List<T>.Enumerator>
    {
        private List<T> source;
        public ListEnumerable(List<T> source) => this.source = source;

        public IEnumerable<T> Source => source;

        public int? TryLength => source.Count;

        public List<T>.Enumerator GetEnumerator() => source.GetEnumerator();

        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TryLast(out T result)
        {
            if (source.Count == 0)
            {
                result = default;
                return false;
            }
            result = source[source.Count - 1];
            return true;
        }
    }
}
