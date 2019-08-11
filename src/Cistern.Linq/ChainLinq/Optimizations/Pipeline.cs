using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IHeadStart<T>
    {
        void Execute(ReadOnlySpan<T> source);
        void Execute(List<T> source);
        void Execute(IList<T> source, int start, int length);
        void Execute(IEnumerable<T> source);
    }

    interface ITailEnd<T>
    {
        void Select<S>(ReadOnlySpan<S> source, Func<S, T> selector);
        void Where(ReadOnlySpan<T> source, Func<T, bool> predicate);
        ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector);
        void WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector);

    }
}
