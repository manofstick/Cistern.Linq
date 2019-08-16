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

        public ImmutableArrayEnumerator<T> GetEnumerator() => new ImmutableArrayEnumerator<T>(source);
    }
}
