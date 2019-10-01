using System.Collections.Generic;

namespace Cistern.Linq.Consume
{
    static class Concat_Deprecated
    {
        public static void Invoke<T>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Chain<T> chain)
        {
            try
            {
                var status = Pipeline(firstOrNull, second, thirdOrNull, chain);

                chain.ChainComplete(status & ~ChainStatus.Flow);
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, ILink<T, V> composition, Chain<V> consumer)
            => Invoke(firstOrNull, second, thirdOrNull, composition.Compose(consumer));

        private static ChainStatus Pipeline<T>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Chain<T> chain)
        {
            UnknownEnumerable.ChainConsumer<T> inner = null;
            ChainStatus status;

            if (firstOrNull != null)
            {
                status = UnknownEnumerable.Consume(firstOrNull, chain, ref inner);
                if (status.IsStopped())
                    return status;
            }

            status = UnknownEnumerable.Consume(second, chain, ref inner);
            if (status.IsStopped())
                return status;

            if (thirdOrNull != null)
            {
                status = UnknownEnumerable.Consume(thirdOrNull, chain, ref inner);
            }

            return status;
        }

    }
}
