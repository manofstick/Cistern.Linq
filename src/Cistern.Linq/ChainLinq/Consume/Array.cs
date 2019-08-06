using System;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class Array
    {
        public static void Invoke<T, V>(T[] array, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipeline<T[]> optimizedForArray)
                {
                    optimizedForArray.Pipeline(array);
                }
                else if(chain is Optimizations.IPipeline<ReadOnlyMemory<T>> optimizedForMemory)
                {
                    optimizedForMemory.Pipeline(array);
                }
                else
                {
                    ReadOnlyMemory.Pipeline(array, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }
    }
}
