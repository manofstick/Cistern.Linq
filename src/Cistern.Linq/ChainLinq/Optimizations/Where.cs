﻿using System;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate);
    }

    interface ITailWhere<T>
    {
        void Where(ReadOnlySpan<T> source, Func<T, bool> predicate);
    }
}
