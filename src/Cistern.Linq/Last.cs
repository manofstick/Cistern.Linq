// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static TSource Last<TSource>(this IEnumerable<TSource> source) =>
            GetLast(source, false);

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source) =>
            GetLast(source, true);

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) =>
            source.Where(predicate).Last();

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) =>
            source.Where(predicate).LastOrDefault();

        private static TSource GetLast<TSource>(IEnumerable<TSource> source, bool orDefault)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Last<TSource>(orDefault));
        }
    }
}
