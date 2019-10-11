using System.Collections.Generic;

namespace Cistern.Linq.Consume
{
    static class List
    {
        const int MinSizeToCoverExecuteCosts = 10; // from some random testing (depends on pipeline length)

        public static void Invoke<T>(List<T> list, Chain<T> chain)
        {
            try
            {
                var status = ChainStatus.Flow;

                if (list.Count > MinSizeToCoverExecuteCosts && chain is Optimizations.IHeadStart<T> optimized)
                {
                    status = optimized.Execute<Optimizations.ListEnumerable<T>, List<T>.Enumerator>(new Optimizations.ListEnumerable<T>(list));
                }
                else
                {
                    status = Pipeline(list, chain);
                }

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static ChainStatus Pipeline<T>(List<T> list, Chain<T> chain)
        {
            var state = ChainStatus.Flow;
            foreach (var item in list)
            {
                state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
            return state;
        }

    }
}
