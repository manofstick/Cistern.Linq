using System;

namespace Cistern.Linq.Optimizations
{
    interface IMergeSelect<T>
    {
        IConsumable<U> MergeSelect<U>(Consumable<T> consumable, Func<T, U> selector);
    }

    interface IMergeWhere<T>
    {
        IConsumable<T> MergeWhere(Consumable<T> consumable, Func<T, bool> predicate);
    }

    interface IMergeSkipTake<T>
    {
        IConsumable<T> MergeSkip(Consumable<T> consumable, int skip);
        IConsumable<T> MergeTake(Consumable<T> consumable, int take);
    }
}
