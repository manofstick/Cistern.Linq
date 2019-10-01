using System;

namespace Cistern.Linq.Optimizations
{
    interface IMergeSelect<T>
    {
        Consumable<U> MergeSelect<U>(Consumable<T> consumable, Func<T, U> selector);
    }

    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(Consumable<T> consumable, Func<T, bool> predicate);
    }

    interface IMergeSkipTake<T>
    {
        Consumable<T> MergeSkip(Consumable<T> consumable, int skip);
        Consumable<T> MergeTake(Consumable<T> consumable, int take);
    }
}
