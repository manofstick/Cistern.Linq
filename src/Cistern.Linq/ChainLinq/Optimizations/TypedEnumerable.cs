using System;
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
    }

    struct IEnumerableEnumerable<T>
        : ITypedEnumerable<T, IEnumerator<T>>
    {
        public IEnumerableEnumerable(IEnumerable<T> source) => Source = source;

        public IEnumerable<T> Source { get; }

        public int? TryLength => null;

        public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();

        public ReadOnlySpan<T> TryGetSourceAsSpan() => null;

        public bool TryGetSourceAsSpan(out ReadOnlySpan<T> readOnlySpan)
        {
            readOnlySpan = default;
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
    }
}
