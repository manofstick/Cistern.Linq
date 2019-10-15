using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.Optimizations
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
        public readonly List<T> List { get; }

        public ListEnumerable(List<T> source) => this.List = source;

        public IEnumerable<T> Source => List;

        public int? TryLength => List.Count;

        public List<T>.Enumerator GetEnumerator() => List.GetEnumerator();

        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
            return false;
        }

        public bool TryLast(out T result)
        {
            if (List.Count == 0)
            {
                result = default;
                return false;
            }
            result = List[List.Count - 1];
            return true;
        }
    }
}
