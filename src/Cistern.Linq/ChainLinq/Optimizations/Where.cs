using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableCons<T> consumable, Func<T, bool> predicate);
    }
}
