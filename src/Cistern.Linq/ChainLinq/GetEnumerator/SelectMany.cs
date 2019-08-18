using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.GetEnumerator
{
    static class SelectMany
    {
        public static IEnumerator<V> Get<Enumerable, T, V>(Consumable<Enumerable> selectMany, Link<T, V> link)
            where Enumerable : IEnumerable<T>
        {
            return new ConsumerEnumerators.SelectMany<Enumerable, T, V>(selectMany, link);
        }

        public static IEnumerator<V> Get<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> selectMany, Func<TSource, TCollection, T> resultSelector, Link<T, V> link)
        {
            return new ConsumerEnumerators.SelectMany<TSource, TCollection, T, V>(selectMany, resultSelector, link);
        }
    }
}
