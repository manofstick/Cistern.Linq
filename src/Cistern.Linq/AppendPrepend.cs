// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            return ChainLinq.Consumables.Append<TSource>.Create(source, element);
        }

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is ChainLinq.Consumables.Concat_Deprecated<TSource, TSource> forPrepending)
            {
                return forPrepending.Prepend(element);
            }

            return new ChainLinq.Consumables.Concat_Deprecated<TSource, TSource>(new ChainLinq.Consumables.Prepender<TSource>(element), source, null, ChainLinq.Links.Identity<TSource>.Instance);
        }

    }
}
