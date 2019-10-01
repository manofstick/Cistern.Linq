// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source) => Distinct(source, null);

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            var distinctLink =
                (comparer == null || ReferenceEquals(comparer, EqualityComparer<TSource>.Default))
                    ? Links.DistinctDefaultComparer<TSource>.Instance
                    : new Links.Distinct<TSource>(comparer);

            return Utils.PushTTTransform(source, distinctLink);
        }
    }
}
