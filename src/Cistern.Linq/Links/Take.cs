using System;

namespace Cistern.Linq.Links
{
    sealed class Take<T>
        : ILink<T, T>
        , Optimizations.ILinkFastCount
        , Optimizations.IMergeSkipTake<T>
    {
        private int _count;

        public Take(int count) =>
            _count = count;

        bool Optimizations.ILinkFastCount.SupportedAsConsumer => true;

        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count) => Math.Min(_count, count);

        public IConsumable<T> MergeSkip(Consumable<T> consumable, int skip)
        {
            if (skip == 0)
                return consumable;

            return consumable.ReplaceTailLink(new TakeSkip<T>(_count, skip));
        }

        public IConsumable<T> MergeTake(Consumable<T> consumable, int take)
        {
            if (take > _count)
                return consumable;

            return consumable.ReplaceTailLink(new Take<T>(take));
        }

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(_count, activity);

        sealed class Activity
            : Activity<T, T>
            , Optimizations.IHeadStart<T>
        {
            private readonly int count;

            private int index;

            public Activity(int count, Chain<T> next) : base(next) =>
                (this.count, index) = (count, 0);

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
}
