// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is ICollection<TSource> collectionoft)
            {
                return collectionoft.Count;
            }

            if (source is ICollection collection)
            {
                return collection.Count;
            }

            var consumable = ChainLinq.Utils.AsConsumable(source);
            if (consumable is ChainLinq.Optimizations.IConsumableFastCount opt)
            {
                var tryCount = opt.TryFastCount(true);
                if (tryCount.HasValue)
                    return tryCount.Value;
            }

            return ChainLinq.Utils.Consume(consumable, new ChainLinq.Consumer.Count<TSource, int, int, double, Maths.OpsInt>());
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => source.Where(predicate).Count();

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Count<TSource, long, long, double, Maths.OpsLong>());
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => source.Where(predicate).Count();
    }
}
