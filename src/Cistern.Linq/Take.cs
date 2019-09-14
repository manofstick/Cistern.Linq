// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return
                count <= 0
                  ? ChainLinq.Consumables.Empty<TSource>.Instance
                  : ChainLinq.Utils.PushTTTransform(source, new ChainLinq.Links.Take<TSource>(count));
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            return ChainLinq.Utils.PushTTTransform(source, new ChainLinq.Links.TakeWhile<TSource>(predicate));
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            return ChainLinq.Utils.PushTTTransform(source, new ChainLinq.Links.TakeWhileIndexed<TSource>(predicate));
        }

        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return count <= 0 ?
                ChainLinq.Consumables.Empty<TSource>.Instance :
                TakeLastDelayed(source, count);
        }

        private static IEnumerable<TSource> TakeLastDelayed<TSource>(IEnumerable<TSource> source, int count)
        {
            var queue = ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.TakeLast<TSource>(count));
            while (queue.Count > 0)
            {
                yield return queue.Dequeue();
            }
        }
    }
}
