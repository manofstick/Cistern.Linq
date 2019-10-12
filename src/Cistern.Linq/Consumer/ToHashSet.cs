using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{
    sealed class ToHashSet<TSource>
        : Consumer<TSource, HashSet<TSource>>
        , Optimizations.IHeadStart<TSource>
    {
        public ToHashSet(IEqualityComparer<TSource> comparer)
            : base(new HashSet<TSource>(comparer)) {}

        ChainStatus Optimizations.IHeadStart<TSource>.Execute(ReadOnlySpan<TSource> source)
        {
            foreach (var item in source)
                Result.Add(item);

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
                Result.Add(item);

            return ChainStatus.Flow;
        }

        public override ChainStatus ProcessNext(TSource input)
        {
            Result.Add(input);

            return ChainStatus.Flow;
        }
    }
}
