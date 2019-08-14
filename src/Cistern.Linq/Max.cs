// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static int Max(this IEnumerable<int> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxInt());
        }

        public static int? Max(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableInt());
        }

        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxLong());
        }

        public static long? Max(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableLong());
        }

        public static double Max(this IEnumerable<double> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDouble());
        }

        public static double? Max(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDouble());
        }

        public static float Max(this IEnumerable<float> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxFloat());
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableFloat());
        }

        public static decimal Max(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDecimal());
        }

        public static decimal? Max(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDecimal());
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (default(TSource) == null)
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxRefType<TSource>());
            }
            else
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxValueType<TSource>());
            }
        }

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Max();

        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Max();

        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Max();

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Max();

        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Max();

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Max();

        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Max();

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Max();

        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Max();

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Max();

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).Max();
    }
}
