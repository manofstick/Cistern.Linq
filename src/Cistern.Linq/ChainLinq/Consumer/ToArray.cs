using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class ToArrayKnownSize<T> : Consumer<T, T[]>
    {
        private int _index;

        public ToArrayKnownSize(int count) : base(new T[count]) =>
            _index = 0;

        public override ChainStatus ProcessNext(T input)
        {
            Result[_index++] = input;
            return ChainStatus.Flow;
        }
    }

    sealed class ToArrayViaBuilder<T> : Consumer<T, T[]>
    {
        List<T> builder;

        public ToArrayViaBuilder() : base(null) =>
            builder = new List<T>();

        public override ChainStatus ProcessNext(T input)
        {
            builder.Add(input);
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = builder.ToArray();
        }
    }
}
