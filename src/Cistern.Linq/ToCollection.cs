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

            int count;
            switch (source)
            {
                case ChainLinq.Consumable<TSource> consumable:
                    if (consumable is ChainLinq.Optimizations.ICountOnConsumable counter)
                    {
                        count = counter.GetCount(true);
                        if (count == 0)
                            return Array.Empty<TSource>();
                        else if (count > 0)
                        {
                            return ChainLinq.Utils.Consume(consumable, new ChainLinq.Consumer.ToArrayKnownSize<TSource>(count));
                        }
                    }

                    return ChainLinq.Utils.Consume(consumable, new ChainLinq.Consumer.ToArrayViaBuilder<TSource>());

                case ICollection<TSource> collection:
                    count = collection.Count;
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
                if (source is ChainLinq.Optimizations.ICountOnConsumable counter)
                {
                    var count = counter.GetCount(true);
                    if (count >= 0)
                    {
                        return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.ToList<TSource>(count));
                    }
                }

                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.ToList<TSource>());
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

            var consumable = ChainLinq.Utils.AsConsumable(source);

            if (consumable is ChainLinq.Optimizations.ICountOnConsumable counter)
            {
                var count = counter.GetCount(true);
                if (count >= 0)
                {
                    var builder = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeySourceSelector<TSource, TKey>, TSource, TKey, TSource>(new ChainLinq.Consumer.KeySourceSelector<TSource, TKey>(keySelector), count, comparer);
                    consumable.Consume(builder);
                    return builder.Result;
                }
            }

            var builder2 = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeySourceSelector<TSource, TKey>, TSource, TKey, TSource>(new ChainLinq.Consumer.KeySourceSelector<TSource, TKey>(keySelector), comparer);
            consumable.Consume(builder2);
            return builder2.Result;
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

            var consumable = ChainLinq.Utils.AsConsumable(source);

            if (consumable is ChainLinq.Optimizations.ICountOnConsumable counter)
            {
                var count = counter.GetCount(true);
                if (count >= 0)
                {
                    var builder = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>, TSource, TKey, TElement>(new ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>(keySelector, elementSelector), count, comparer);
                    consumable.Consume(builder);
                    return builder.Result;
                }
            }

            var builder2 = new ChainLinq.Consumer.ToDictionary<ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>, TSource, TKey, TElement>(new ChainLinq.Consumer.KeyElementSelector<TSource, TKey, TElement>(keySelector, elementSelector), comparer);
            consumable.Consume(builder2);
            return builder2.Result;
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
