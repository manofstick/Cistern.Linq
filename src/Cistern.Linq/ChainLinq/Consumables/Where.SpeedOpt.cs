using Cistern.Linq.ChainLinq.Optimizations;
using System;

namespace Cistern.Linq.ChainLinq.Consumables
{
    sealed partial class WhereArray<T> : IMergeSelect<T>, IMergeWhere<T>
    {
        Consumable<V> IMergeSelect<T>.MergeSelect<V>(ConsumableForMerging<T> _, Func<T, V> u2v) =>
            new WhereSelectArray<T, V>(Underlying, Predicate, u2v);

        Consumable<T> IMergeWhere<T>.MergeWhere(ConsumableForMerging<T> _, Func<T, bool> predicate) =>
            new WhereArray<T>(Underlying, t => Predicate(t) && predicate(t));
    }

    sealed partial class WhereEnumerable<TEnumerable, TEnumerator, T> : IMergeSelect<T>, IMergeWhere<T>
    {
        Consumable<V> IMergeSelect<T>.MergeSelect<V>(ConsumableForMerging<T> consumable, Func<T, V> u2v) =>
            new WhereSelectEnumerable<TEnumerable, TEnumerator, T, V>(Underlying, Predicate, u2v);

        Consumable<T> IMergeWhere<T>.MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate) =>
            new WhereEnumerable<TEnumerable, TEnumerator, T>(Underlying, t => Predicate(t) && predicate(t));
    }
}
