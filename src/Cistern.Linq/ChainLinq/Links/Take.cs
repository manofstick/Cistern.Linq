using System;

namespace Cistern.Linq.ChainLinq.Links
{
    sealed class Take<T>
        : ActivityLink<T, T>
        , Optimizations.ICountOnConsumableLink
        , Optimizations.IHeadStart<T>
    {
        private readonly int count;

        private int index;

        public Take(int count) => (this.count, this.index) = (count, 0);

        internal override ActivityLink<T, T> Clone() => new Take<T>(count);

        int Optimizations.ICountOnConsumableLink.GetCount(int count)
        {
            checked
            {
                return Math.Min(this.count, count);
            }
        }

        public override ChainStatus ProcessNext(T input)
        {
            if (index >= count)
            {
                return ChainStatus.Stop;
            }

            checked
            {
                index++;
            }

            if (index >= count)
                return ChainStatus.Stop | Next(input);
            else
                return Next(input);
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            if (count < source.Length)
                source = source.Slice(0, count);

            if (next is Optimizations.IHeadStart<T> optimizations)
                return optimizations.Execute(source);
            else
            {
                foreach (var item in source)
                {
                    var status = Next(item);
                    if (status.IsStopped())
                        return status;
                }
                return ChainStatus.Flow;
            }
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var input in source)
            {
                checked
                {
                    index++;
                }

                if (index >= count)
                    return ChainStatus.Stop | Next(input);

                var status = Next(input);
                if (status.IsStopped())
                    return status;
            }
            return ChainStatus.Flow;
        }
    }
}
