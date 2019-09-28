// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            switch (source)
            {
                case ChainLinq.Consumable<TSource> consumable:
                    ChainLinq.Consumer<TSource, TSource[]> toArray = null;

                    if (consumable is ChainLinq.Optimizations.IConsumableFastCount counter)
                    {
                        var tryCount = counter.TryFastCount(false);
                        if (tryCount.HasValue)
                        {
                            if (tryCount.Value == 0)
                                return Array.Empty<TSource>();
                            else
                                toArray = new ChainLinq.Consumer.ToArrayKnownSize<TSource>(tryCount.Value);
                        }
                    }

                    toArray ??= new ChainLinq.Consumer.ToArrayViaBuilder<TSource>();

                    return ChainLinq.Utils.Consume(consumable, toArray);

                case ICollection<TSource> collection:
                    var count = collection.Count;
                    if (count == 0)
                        return Array.Empty<TSource>();
                    else
                    {
                        var result = new TSource[count];
                        collection.CopyTo(result, 0);
                        return result;
                    }

                default:
                    return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.ToArrayViaBuilder<TSource>());
            }
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is ChainLinq.Consumable<TSource> consumable)
            {
                ChainLinq.Consumer<TSource, List<TSource>> toList = null;

                if (source is ChainLinq.Optimizations.IConsumableFastCount counter)
                {
                    var tryCount = counter.TryFastCount(false);
                    if (tryCount.HasValue)
                        toList = new ChainLinq.Consumer.ToList<TSource>(tryCount.Value);
                }

                toList ??= new ChainLinq.Consumer.ToList<TSource>();

                return ChainLinq.Utils.Consume(source, toList);
            }

            return new List<TSource>(source);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            ToDictionary(source, keySelector, null);

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            ChainLinq.Consumer<TSource, Dictionary<TKey, TSource>> toDictionary = null;

            var consumable = ChainLinq.Utils.AsConsumable(source);
            if (consumable is ChainLinq.Optimizations.IConsumableFastCount counter)
            {
                var tryCount = counter.TryFastCount(false);
                if (tryCount.HasValue)
                    toDictionary = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeySourceSelector<TSource, TKey>, TSource, TKey, TSource>(new ChainLinq.Consumer.KeySourceSelector<TSource, TKey>(keySelector), tryCount.Value, comparer);
            }

            toDictionary ??= new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeySourceSelector<TSource, TKey>, TSource, TKey, TSource>(new ChainLinq.Consumer.KeySourceSelector<TSource, TKey>(keySelector), comparer);

            return ChainLinq.Utils.Consume(source, toDictionary);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
            ToDictionary(source, keySelector, elementSelector, null);

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (keySelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
            }

            if (elementSelector == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
            }

            ChainLinq.Consumer<TSource, Dictionary<TKey, TElement>> toDictionary = null;

            var consumable = ChainLinq.Utils.AsConsumable(source);
            if (consumable is ChainLinq.Optimizations.IConsumableFastCount counter)
            {
                var tryCount = counter.TryFastCount(false);
                if (tryCount.HasValue)
                    toDictionary = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>, TSource, TKey, TElement>(new ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>(keySelector, elementSelector), tryCount.Value, comparer);
            }

            toDictionary ??= new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>, TSource, TKey, TElement>(new ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>(keySelector, elementSelector), comparer);

            return ChainLinq.Utils.Consume(source, toDictionary);
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(comparer: null);

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            // Don't pre-allocate based on knowledge of size, as potentially many elements will be dropped.
            return new HashSet<TSource>(source, comparer);
        }
    }
}
