using System;

namespace Cistern.Linq.Optimizations
{
    interface IMergeSelect<T>
    {
        IConsumable<U> MergeSelect<U>(IConsumable<T> consumable, Func<T, U> selector);
    }

    interface IMergeWhere<T>
    {
        IConsumable<T> MergeWhere(IConsumable<T> consumable, Func<T, bool> predicate);
    }

    interface IMergeSkipTake<T>
    {
        IConsumable<T> MergeSkip(IConsumable<T> consumable, int skip);
        IConsumable<T> MergeTake(IConsumable<T> consumable, int take);
    }
}
