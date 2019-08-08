// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static int Sum(this IEnumerable<int> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumInt());
        }

        public static int? Sum(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableInt());
        }

        public static long Sum(this IEnumerable<long> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumLong());
        }

        public static long? Sum(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableLong());
        }

        public static float Sum(this IEnumerable<float> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumFloat());
        }

        public static float? Sum(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableFloat());
        }

        public static double Sum(this IEnumerable<double> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDouble());
        }

        public static double? Sum(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDouble());
        }

        public static decimal Sum(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDecimal());
        }

        public static decimal? Sum(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDecimal());
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Sum();
        public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Sum();
        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Sum();
        public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Sum();
        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Sum();
        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Sum();
        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Sum();
        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Sum();
        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Sum();
        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Sum();
    }
}
