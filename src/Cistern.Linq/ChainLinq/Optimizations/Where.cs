using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate);
    }

    interface IWhereArray
    {
        void Where<T>(T[] array, Func<T, bool> predicate);
    }
}
