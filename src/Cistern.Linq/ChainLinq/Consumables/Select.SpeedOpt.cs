using Cistern.Linq.ChainLinq.Optimizations;
using System;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class SelectArray<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectArray<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> _, Func<U, bool> predicate) =>
            new SelectWhereArray<T, U>(Underlying, Selector, predicate);
    }

    sealed partial class SelectEnumerable<TEnumerable, TEnumerator, T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> _, Func<U, bool> predicate) =>
            new SelectWhereEnumerable<TEnumerable, TEnumerator, T, U>(Underlying, Selector, predicate);
    }
}
