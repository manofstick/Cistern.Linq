using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeSelect<T>
    {
        Consumable<U> MergeSelect<U>(ConsumableCons<T> consumable, Func<T, U> selector);
    }

    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableCons<T> consumable, Func<T, bool> predicate);
    }

    interface IMergeSkipTake<T>
    {
        Consumable<T> MergeSkip(ConsumableCons<T> consumable, int skip);
        Consumable<T> MergeTake(ConsumableCons<T> consumable, int take);
    }
}
