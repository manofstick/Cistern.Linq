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
            return source switch
            {
                null                            => ThrowHelper.ThrowArgumentNullException<TSource[]>(ExceptionArgument.source),
                ICollection<TSource> collection => ForCollection(collection),
                IConsumable<TSource> consumable => ForConsumable(consumable),
                _                               => Utils.ToArray(source)
            };

            static TSource[] ForCollection(ICollection<TSource> collection)
            {
                var count = collection.Count;

                if (count == 0)
                {
                    return Array.Empty<TSource>();
                }
                else
                {
                    var result = new TSource[count];
                    collection.CopyTo(result, 0);
                    return result;
                }
            }

            static TSource[] ForConsumable(IConsumable<TSource> consumable)
            {
                Consumer<TSource, TSource[]> toArray = null;

                if (consumable is Optimizations.IDelayed<TSource> delayed)
                {
                    consumable = delayed.Force();
                }

                if (consumable is Optimizations.ITryGetCollectionInterface<TSource> tgci && tgci.TryGetCollectionInterface(out var asCollection))
                {
                    return ForCollection(asCollection);
                }

                if (consumable is Optimizations.IConsumableFastCount counter)
                {
                    var tryCount = counter.TryFastCount(false);
                    if (tryCount.HasValue)
                    {
                        if (tryCount.Value == 0)
                        {
                            return Array.Empty<TSource>();
                        }
                        else
                        {
                            toArray = new Consumer.ToArrayKnownSize<TSource>(tryCount.Value);
                        }
                    }
                }

                toArray ??= new Consumer.ToArrayViaBuilder<TSource>();

                return Utils.Consume(consumable, toArray);
            }
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            // NB. Optimizations here rely on knowledge that the constructor of List optimizes for the ICollection
            // interface, which makes it the fastest method for constructing a List.
            switch (source)
            {
                case null:
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
                    goto default;

                case ICollection<TSource> _:
                    goto default;

                case Optimizations.ITryGetCollectionInterface<TSource> tgci when tgci.TryGetCollectionInterface(out var asCollection):
                    source = asCollection;
                    goto default;

                case IConsumable<TSource> consumable:
                    Consumer<TSource, List<TSource>> toList = null;

                    if (consumable is Optimizations.IDelayed<TSource> delayed)
                        consumable = delayed.Force();

                    if (consumable is Optimizations.IConsumableFastCount counter)
                    {
                        var tryCount = counter.TryFastCount(false);
                        if (tryCount.HasValue)
                            toList = new Consumer.ToList<TSource>(tryCount.Value);
                    }

                    toList ??= new Consumer.ToList<TSource>();

                    return Utils.Consume(consumable, toList);

                default:
                    return new List<TSource>(source);
            }
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

            var consumable = GetFinalizedConsumable(source);

            var toDictionary = GetDictionaryConsumer(consumable, keySelector, elementSelector, comparer);

            return Utils.Consume(consumable, toDictionary);
        }

        private static Consumer<TSource, Dictionary<TKey, TElement>> GetDictionaryConsumer<TSource, TKey, TElement>(IConsumable<TSource> consumable, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (consumable is Optimizations.IConsumableFastCount counter)
            {
                var tryCount = counter.TryFastCount(false);
                if (tryCount.HasValue)
                    return new Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, tryCount.Value, comparer);
            }

            return new Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, comparer);
        }

        private static IConsumable<TSource> GetFinalizedConsumable<TSource>(IEnumerable<TSource> source)
        {
            var consumable = Utils.AsConsumable(source);
            if (consumable is Optimizations.IDelayed<TSource> delayed)
                consumable = delayed.Force();
            return consumable;
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(comparer: null);

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return source switch
            {
                null => ThrowHelper.ThrowArgumentNullException<HashSet<TSource>>(ExceptionArgument.source),
                IConsumable<TSource> consumable => Utils.Consume(consumable, new Consumer.ToHashSet<TSource>(comparer)),
                _ => new HashSet<TSource>(source, comparer),
            };
        }
    }
}
