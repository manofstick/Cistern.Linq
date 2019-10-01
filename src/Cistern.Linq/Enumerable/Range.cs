// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Cistern.Linq
{
    public static partial class Enumerable
    {
        static readonly ILink<int, int> IntIdentity = Links.Identity<int>.Instance;

        public static IEnumerable<int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
            }

            if (count == 0)
            {
                return Consumables.Empty<int>.Instance;
            }

            return new Consumables.Range<int>(start, count, IntIdentity);
        }
    }
}
