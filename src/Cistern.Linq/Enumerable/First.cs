// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        private static TSource GetFirst<TSource>(IEnumerable<TSource> source, bool orDefault)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.First<TSource>(orDefault));
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source) => GetFirst(source, false);

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => GetFirst(source.Where(predicate), false);

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source) => GetFirst(source, true);

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => GetFirst(source.Where(predicate), true);
    }
}
