using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeSelect<T>
    {
        Consumable<U> MergeSelect<U>(ConsumableForMerging<T> consumable, Func<T, U> selector);
    }

    interface ITailSelect<T>
    {
        void Select<S> (ReadOnlySpan<S> source, Func<S, T> selector);
    }

}
