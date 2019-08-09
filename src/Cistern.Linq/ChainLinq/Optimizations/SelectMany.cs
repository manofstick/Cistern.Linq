using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ISelectMany<T>
    {
        ChainStatus SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector);
    }

}
