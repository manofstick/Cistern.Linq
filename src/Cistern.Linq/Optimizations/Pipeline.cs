using System;
using System.Collections.Generic;

namespace Cistern.Linq.Optimizations
{
    interface IHeadStart<T>
    {
        ChainStatus Execute(ReadOnlySpan<T> source);
        ChainStatus Execute<Enumerable, Enumerator>(Enumerable source)
            where Enumerable : ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>;
    }

    interface ITailEnd<T>
    {
        ChainStatus Select<S>(ReadOnlySpan<S> source, Func<S, T> selector);
        ChainStatus Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
            where Enumerable : ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>;

        ChainStatus Where(ReadOnlySpan<T> source, Func<T, bool> predicate);
        ChainStatus Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
            where Enumerable : ITypedEnumerable<T, Enumerator>
            where Enumerator : IEnumerator<T>;

        ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector);

        ChainStatus WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector);
        ChainStatus WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
            where Enumerable : ITypedEnumerable<S, Enumerator>
            where Enumerator : IEnumerator<S>;

    }
}
