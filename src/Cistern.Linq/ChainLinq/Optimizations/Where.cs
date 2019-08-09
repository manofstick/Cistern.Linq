using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate);
    }

    interface IWhereArray<T>
    {
        void Where(T[] array, Func<T, bool> predicate);
    }
}
