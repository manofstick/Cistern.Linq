// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
            }

            if (second == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
            }

            return ExceptConsumer(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
            }

            if (second == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
            }

            return ExceptConsumer(first, second, comparer);
        }

        private static IEnumerable<TSource> ExceptConsumer<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            ChainLinq.ILink<TSource, TSource> exceptLink =
                (comparer == null || ReferenceEquals(comparer, EqualityComparer<TSource>.Default))
                    ? (ChainLinq.ILink<TSource, TSource>) new ChainLinq.Links.ExceptDefaultComparer<TSource>(second)
                    : (ChainLinq.ILink<TSource, TSource>) new ChainLinq.Links.Except<TSource>(comparer, second);

            return ChainLinq.Utils.PushTTTransform(first, exceptLink);
        }
    }
}
