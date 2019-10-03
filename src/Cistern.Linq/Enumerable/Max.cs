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

            using var consumer = Consumer.MaxInt.FactoryCreate(); 
            return Utils.Consume(source, consumer);
        }

        public static int? Max(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<int, Consumer.MaxNullableLogic<int, int, double, Maths.OpsInt>>());
        }

        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            using var consumer = Consumer.MaxLong.FactoryCreate(); 
            return Utils.Consume(source, consumer);
        }

        public static long? Max(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<long, Consumer.MaxNullableLogic<long, long, double, Maths.OpsLong>>());
        }

        public static double Max(this IEnumerable<double> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            using var consumer = Consumer.MaxDouble.FactoryCreate(); 
            return Utils.Consume(source, consumer);
        }

        public static double? Max(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<double, Consumer.MaxNullableLogic<double, double, double, Maths.OpsDouble>>());
        }

        public static float Max(this IEnumerable<float> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            using var consumer = Consumer.MaxFloat.FactoryCreate();
            return Utils.Consume(source, consumer);
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<float, Consumer.MaxNullableLogic<float, double, float, Maths.OpsFloat>>());
        }

        public static decimal Max(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            using var consumer = Consumer.MaxDecimal.FactoryCreate();
            return Utils.Consume(source, consumer);
        }

        public static decimal? Max(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return Utils.Consume(source, new Consumer.GenericNullable<decimal, Consumer.MaxNullableLogic<decimal, decimal, decimal, Maths.OpsDecimal>>());
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (default(TSource) == null)
            {
                return Utils.Consume(source, new Consumer.MaxRefType<TSource>());
            }
            else
            {
                return Utils.Consume(source, new Consumer.MaxValueType<TSource>());
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
