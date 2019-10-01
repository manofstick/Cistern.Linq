using System;

namespace Cistern.Linq.Links
{
    sealed class TakeSkip<T>
        : ILink<T, T>
        , Optimizations.ILinkFastCount
        , Optimizations.IMergeSkipTake<T>
    {
        private int _take;
        private int _skip;

        public TakeSkip(int take, int skip) => (_take, _skip) = (take, skip);

        bool Optimizations.ILinkFastCount.SupportedAsConsumer => true;

        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count)
        {
            checked
            {
                return Math.Min(Math.Max(0, _take - _skip), count);
            }
        }

        public Consumable<T> MergeSkip(ConsumableCons<T> consumable, int skip)
        {
            checked
            {
                if (skip == 0)
                    return consumable;

                // trying to skip more than MaxValue is a no-op, as take can only have
                // a maximum of int.Value
                var totalSkip = (int)Math.Min(int.MaxValue, (long)_skip + skip);

                return consumable.ReplaceTailLink(new TakeSkip<T>(_take, totalSkip));
            }
        }

        public Consumable<T> MergeTake(ConsumableCons<T> consumable, int take)
        {
            checked
            {
                if (_skip == 0)
                {
                    if (take >= _take)
                        return consumable;

                    return consumable.ReplaceTailLink(new Take<T>(take));
                }

                return consumable.ReplaceTailLink(Composition.Create(this, new Take<T>(take)));
            }
        }

        Chain<T> ILink<T,T>.Compose(Chain<T> activity) =>
            new Activity(_take, _skip, activity);

        sealed class Activity
            : Activity<T, T>
            , Optimizations.IHeadStart<T>
        {
            private readonly int take;
            private readonly int skip;

            private int index;

            public Activity(int take, int skip, Chain<T> next) : base(next)
            {
                (this.take, this.skip) = (take, Math.Min(skip, take));
                index = 0;
            }

            public override ChainStatus ProcessNext(T input)
            {
                if (index >= take || skip >= take)
                {
                    return ChainStatus.Stop;
                }

                checked
                {
                    index++;
                }

                if (index <= skip)
                    return ChainStatus.Filter;
                else if (index >= take)
                    return ChainStatus.Stop | Next(input);
                else
                    return Next(input);
            }

            ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
            {
                if (take < source.Length)
                    source = source.Slice(skip, Math.Max(0, take-skip));

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
                if (skip >= take)
                    return ChainStatus.Filter;

                foreach (var input in source)
                {
                    checked
                    {
                        index++;
                    }

                    if (index <= skip)
                        continue;

                    if (index >= take)
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
