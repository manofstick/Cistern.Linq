using System;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class ReadOnlySpan
    {
        const int MinSizeToCoverExecuteCosts = 5; // from some random testing (depends on pipeline length)

        public static void Invoke<T>(ReadOnlySpan<T> array, Chain<T> chain)
        {
            try
            {
                var status = ChainStatus.Flow;

                if (array.Length > MinSizeToCoverExecuteCosts && chain is Optimizations.IHeadStart<T> optimized)
                {
                    status = optimized.Execute(array);
                }
                else
                {
                    status = Pipeline(array, chain);
                }

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(ReadOnlySpan<T> array, ILink<T, V> composition, Chain<V> consumer) =>
            Invoke(array, composition.Compose(consumer));

        internal static ChainStatus Pipeline<T>(ReadOnlySpan<T> memory, Chain<T> chain)
        {
            var state = ChainStatus.Flow;
            foreach (var item in memory)
            {
                state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
            return state;
        }

    }
}
