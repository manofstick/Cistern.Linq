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
                case IConsumable<TSource> consumable:
                    Consumer<TSource, TSource[]> toArray = null;

                    if (consumable is Optimizations.IDelayed<TSource> delayed)
                        consumable = delayed.Force();

                    if (consumable is Optimizations.IConsumableFastCount counter)
                    {
                        var tryCount = counter.TryFastCount(false);
                        if (tryCount.HasValue)
                        {
                            if (tryCount.Value == 0)
                                return Array.Empty<TSource>();
                            else
                                toArray = new Consumer.ToArrayKnownSize<TSource>(tryCount.Value);
                        }
                    }

                    toArray ??= new Consumer.ToArrayViaBuilder<TSource>();

                    return Utils.Consume(consumable, toArray);

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
                    return Utils.Consume(source, new Consumer.ToArrayViaBuilder<TSource>());
            }
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is Optimizations.ITryGetCollectionInterface<TSource> tgci && tgci.TryGetCollectionInterface(out var asCollection))
            {
                return new List<TSource>(asCollection);
            }

            if (source is IConsumable<TSource> consumable)
            {
                Consumer<TSource, List<TSource>> toList = null;

                if (consumable is Optimizations.IDelayed<TSource> delayed)
                    consumable = delayed.Force();

                if (source is Optimizations.IConsumableFastCount counter)
                {
                    var tryCount = counter.TryFastCount(false);
                    if (tryCount.HasValue)
                        toList = new Consumer.ToList<TSource>(tryCount.Value);
                }

                toList ??= new Consumer.ToList<TSource>();

                return Utils.Consume(consumable, toList);
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

            Consumer<TSource, Dictionary<TKey, TSource>> toDictionary = null;

            var consumable = Utils.AsConsumable(source);

            if (consumable is Optimizations.IDelayed<TSource> delayed)
                consumable = delayed.Force();

            if (consumable is Optimizations.IConsumableFastCount counter)
            {
                var tryCount = counter.TryFastCount(false);
                if (tryCount.HasValue)
                    toDictionary = new Consumer.ToDictionary<TSource, TKey, TSource>(keySelector, x => x, tryCount.Value, comparer);
            }

            toDictionary ??= new Consumer.ToDictionary<TSource, TKey, TSource>(keySelector, x => x, comparer);

            return Utils.Consume(consumable, toDictionary);
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

            Consumer<TSource, Dictionary<TKey, TElement>> toDictionary = null;

            var consumable = Utils.AsConsumable(source);

            if (consumable is Optimizations.IDelayed<TSource> delayed)
                consumable = delayed.Force();

            if (consumable is Optimizations.IConsumableFastCount counter)
            {
                var tryCount = counter.TryFastCount(false);
                if (tryCount.HasValue)
                    toDictionary = new Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, tryCount.Value, comparer);
            }

            toDictionary ??= new Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, comparer);

            return Utils.Consume(consumable, toDictionary);
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
