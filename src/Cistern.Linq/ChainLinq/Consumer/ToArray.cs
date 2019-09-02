using Cistern.Linq.Utils;
using System;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class ToArrayKnownSize<T>
        : Consumer<T, T[]>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
    {
        private int _index;

        public ToArrayKnownSize(int count) : base(new T[count]) =>
            _index = 0;

        public override ChainStatus ProcessNext(T input)
        {
            Result[_index++] = input;
            return ChainStatus.Flow;
        }

        void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            source.CopyTo(new Span<T>(Result, _index, Result.Length-_index));
            _index += source.Length;
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
            {
                Result[_index++] = item;
            }
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                Result[_index++] = selector(item);
            }
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = item;
                }
            }
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = item;
                }
            }
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var item in span)
            {
                Result[_index++] = resultSelector(source, item);
            }
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = selector(item);
                }
            }
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = selector(item);
                }
            }
        }
    }


    sealed class ToArrayViaBuilder<T>
        : Consumer<T, T[]>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
    {
        ArrayBuilder<T> builder;

        public ToArrayViaBuilder() : base(null) =>
            builder = new ArrayBuilder<T>();

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
            {
                builder.Add(item);
            }
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
            {
                builder.Add(item);
            }
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                builder.Add(selector(item));
            }
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var item in span)
            {
                builder.Add(resultSelector(source, item));
            }
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
        }
    }
}
