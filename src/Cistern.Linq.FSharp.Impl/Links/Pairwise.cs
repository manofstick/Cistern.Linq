using Cistern.Linq;
using System;

namespace Cistern.Linq.FSharp.Links
{
    internal sealed class PairwiseActivity<T>
        : Activity<T, Tuple<T, T>>
        , Optimizations.IHeadStart<T>
    {
        private Tuple<T,T> _previous;

        public PairwiseActivity(Chain<Tuple<T, T>> next) : base(next) { }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            var status = ChainStatus.Flow;
            foreach (var input in source)
            {
                if (_previous == null)
                {
                    _previous = Tuple.Create(input, input);
                }
                else
                {
                    _previous = Tuple.Create(_previous.Item2, input);
                    status = Next(_previous);
                    if (status.IsStopped())
                        break;
                }
            }
            return status;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            var status = ChainStatus.Flow;
            foreach (var input in source)
            {
                if (_previous == null)
                {
                    _previous = Tuple.Create(input, input);
                }
                else
                {
                    _previous = Tuple.Create(_previous.Item2, input);
                    status = Next(_previous);
                    if (status.IsStopped())
                        break;
                }
            }
            return status;
        }

        public override ChainStatus ProcessNext(T input)
        {
            if (_previous == null)
            {
                _previous = Tuple.Create(input, input);
                return ChainStatus.Filter;
            }
            else
            {
                _previous = Tuple.Create(_previous.Item2, input);
                return Next(_previous);
            }
        }
    }

    internal sealed class Pairwise<T>
        : ILink<T, Tuple<T, T>>
        , Optimizations.ILinkFastCount
    {
        static internal ILink<T, Tuple<T, T>> Instance { get; } = new Pairwise<T>();

        bool Optimizations.ILinkFastCount.SupportedAsConsumer => true;
        int? Optimizations.ILinkFastCount.FastCountAdjustment(int count) => Math.Max(0, count - 1);

        private Pairwise() {}

        Chain<T> ILink<T, Tuple<T, T>>.Compose(Chain<Tuple<T, T>> activity) => new PairwiseActivity<T>(activity);
    }
}
