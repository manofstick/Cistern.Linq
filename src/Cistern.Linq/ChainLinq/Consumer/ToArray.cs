using System;
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

    sealed class ToArrayViaBuilder<T>
        : Consumer<T, T[]>
        , Optimizations.IHeadStart<T>

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

        void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            foreach (var item in source)
                builder.Add(item);
        }

        void Optimizations.IHeadStart<T>.Execute(List<T> source)
        {
            foreach (var item in source)
                builder.Add(item);
        }

        void Optimizations.IHeadStart<T>.Execute(IList<T> source, int start, int length)
        {
            for (var i = start; i < start + length; ++i)
                builder.Add(source[i]);
        }

        void Optimizations.IHeadStart<T>.Execute(IEnumerable<T> source)
        {
            foreach (var item in source)
                builder.Add(item);
        }
    }
}
