using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class UnknownEnumerable
    {
        public sealed class ChainConsumer<T> : Consumer<T, ChainStatus>
        {
            private readonly Chain<T> _chainT;

            public ChainConsumer(Chain<T> chainT) : base(ChainStatus.Flow) =>
                _chainT = chainT;

            public override ChainStatus ProcessNext(T input)
            {
                var status = _chainT.ProcessNext(input);
                Result = status;
                return status;
            }
        }

        private static ChainConsumer<T> GetInnerConsumer<T>(Chain<T> chain, ref ChainConsumer<T> consumer) =>
            consumer ?? (consumer = new ChainConsumer<T>(chain));

        public static ChainStatus Consume<T>(IEnumerable<T> input, Chain<T> chain, ref ChainConsumer<T> consumer)
        {
            if (input is Consumable<T> consumable)
            {
                var c = GetInnerConsumer(chain, ref consumer);
                consumable.Consume(c);
                return c.Result;
            }
            else if (input is T[] array)
            {
                return ConsumerSpan(array, chain);
            }
            else if (input is List<T> list)
            {
                return ConsumerList(list, chain);
            }
            else
            {
                return ConsumerEnumerable(input, chain);
            }
        }

        private static ChainStatus ConsumerEnumerable<T>(IEnumerable<T> source, Chain<T> chain)
        {
            if (chain is Optimizations.IHeadStart<T> optimized)
            {
                optimized.Execute(new Optimizations.IEnumerableEnumerable<T>(source));
                return ChainStatus.Flow;
            }
            else
            {
                var status = ChainStatus.Flow;
                foreach (var item in source)
                {
                    status = chain.ProcessNext(item);
                    if (status.IsStopped())
                        break;
                }
                return status;
            }
        }

        private static ChainStatus ConsumerSpan<T>(ReadOnlySpan<T> source, Chain<T> chain)
        {
            if (chain is Optimizations.IHeadStart<T> optimized)
            {
                optimized.Execute(source);
                return ChainStatus.Flow;
            }
            else
            {
                var status = ChainStatus.Flow;
                foreach (var item in source)
                {
                    status = chain.ProcessNext(item);
                    if (status.IsStopped())
                        break;
                }
                return status;
            }
        }

        private static ChainStatus ConsumerList<T>(List<T> source, Chain<T> chain)
        {
            if (chain is Optimizations.IHeadStart<T> optimized)
            {
                optimized.Execute(new Optimizations.ListEnumerable<T>(source));
                return ChainStatus.Flow;
            }
            else
            {
                var status = ChainStatus.Flow;
                foreach (var item in source)
                {
                    status = chain.ProcessNext(item);
                    if (status.IsStopped())
                        break;
                }
                return status;
            }
        }
    }
}
