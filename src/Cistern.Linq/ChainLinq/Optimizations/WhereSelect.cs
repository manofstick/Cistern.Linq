using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface ITailWhereSelect<T>
    {
        void WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector);
    }
}
