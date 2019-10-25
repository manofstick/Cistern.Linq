// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static System.Linq.IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            new Consumables.OrderBy<TSource, TKey>(source, keySelector, null, false, null);

        public static System.Linq.IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            new Consumables.OrderBy<TSource, TKey>(source, keySelector, comparer, false, null);

        public static System.Linq.IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            new Consumables.OrderBy<TSource, TKey>(source, keySelector, null, true, null);

        public static System.Linq.IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            new Consumables.OrderBy<TSource, TKey>(source, keySelector, comparer, true, null);

        public static System.Linq.IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return source.CreateOrderedEnumerable(keySelector, null, false);
        }

        public static System.Linq.IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return source.CreateOrderedEnumerable(keySelector, comparer, false);
        }

        public static System.Linq.IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return source.CreateOrderedEnumerable(keySelector, null, true);
        }

        public static System.Linq.IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return source.CreateOrderedEnumerable(keySelector, comparer, true);
        }
    }
}
