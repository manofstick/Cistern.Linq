using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class Concat
    {
        public static void Invoke<T>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Chain<T> chain)
        {
            try
            {
                Pipeline(firstOrNull, second, thirdOrNull, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Link<T, V> composition, Chain<V> consumer)
            => Invoke(firstOrNull, second, thirdOrNull, composition.Compose(consumer));

        private static void Pipeline<T>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Chain<T> chain)
        {
            UnknownEnumerable.ChainConsumer<T> inner = null;
            ChainStatus status;

            if (firstOrNull != null)
            {
                status = UnknownEnumerable.Consume(firstOrNull, chain, ref inner);
                if (status.IsStopped())
                    return;
            }

            status = UnknownEnumerable.Consume(second, chain, ref inner);
            if (status.IsStopped())
                return;

            if (thirdOrNull != null)
            {
                UnknownEnumerable.Consume(thirdOrNull, chain, ref inner);
            }
        }

    }
}
