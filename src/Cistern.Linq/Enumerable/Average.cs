// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static double Average(this IEnumerable<int> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageInt());
        }

        public static double? Average(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageNullableInt());
        }

        public static double Average(this IEnumerable<long> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageLong());
        }

        public static double? Average(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageNullableLong());
        }

        public static float Average(this IEnumerable<float> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageFloat());
        }

        public static float? Average(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageNullableFloat());
        }

        public static double Average(this IEnumerable<double> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageDouble());
        }

        public static double? Average(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageNullableDouble());
        }

        public static decimal Average(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageDecimal());
        }

        public static decimal? Average(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.AverageNullableDecimal());
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Average();
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Average();
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Average();
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Average();
        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Average();
        public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Average();
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Average();
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Average();
        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Average();
        public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Average();
    }
}
