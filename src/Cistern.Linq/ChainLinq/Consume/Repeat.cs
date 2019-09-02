namespace Cistern.Linq.ChainLinq.Consume
{
    static class Repeat
    {
        public static void Invoke<T>(T element, int count, Chain<T> chain)
        {
            try
            {
                Pipeline(element, count, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<T, V>(T element, int count, Link<T, V> composition, Chain<V> consumer) =>
            Invoke(element, count, composition.Compose(consumer));

        private static void Pipeline<T>(T element, int count, Chain<T> chain)
        {
            for(var i=0; i < count; ++i)
            {
                var state = chain.ProcessNext(element);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
