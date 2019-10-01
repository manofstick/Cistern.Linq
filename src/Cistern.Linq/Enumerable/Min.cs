// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static int Min(this IEnumerable<int> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.MinInt());
        }

        public static int? Min(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<int, Consumer.MinNullableLogic<int, int, double, Maths.OpsInt>>());
        }

        public static long Min(this IEnumerable<long> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.MinLong());
        }

        public static long? Min(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<long, Consumer.MinNullableLogic<long, long, double, Maths.OpsLong>>());
        }

        public static float Min(this IEnumerable<float> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.MinFloat());
        }

        public static float? Min(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<float, Consumer.MinNullableLogic<float, double, float, Maths.OpsFloat>>());
        }

        public static double Min(this IEnumerable<double> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.MinDouble());
        }

        public static double? Min(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<double, Consumer.MinNullableLogic<double, double, double, Maths.OpsDouble>>());
        }

        public static decimal Min(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.MinDecimal());
        }

        public static decimal? Min(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<decimal, Consumer.MinNullableLogic<decimal, decimal, decimal, Maths.OpsDecimal>>());
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (default(TSource) == null)
            {
                return Utils.Consume(source, new Consumer.MinRefType<TSource>());
            }
            else
            {
                return Utils.Consume(source, new Consumer.MinValueType<TSource>());
            }
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Min();
        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Min();
        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Min();
        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Min();
        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Min();
        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Min();
        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Min();
        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Min();
        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Min();
        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Min();
        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).Min();
    }
}
