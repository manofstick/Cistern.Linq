using System;

namespace Cistern.Linq.Consumer
{
    static class ToArrayHelper
    {
        public static void Select<S, T>(ReadOnlySpan<S> source, Func<S, T> selector, T[] result)
        {
            for (int idx = 0; idx < source.Length && idx < result.Length; ++idx)
            {
                result[idx] = selector(source[idx]);
            }
        }

        public static void SelectNonZeroIndex<S, T>(ReadOnlySpan<S> source, Func<S, T> selector, T[] result, int rIdx)
        {
            for (int sIdx = 0; sIdx < source.Length && rIdx < result.Length; ++sIdx, ++rIdx)
            {
                result[rIdx] = selector(source[sIdx]);
            }
        }
    }

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

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            source.CopyTo(new Span<T>(Result, _index, Result.Length-_index));
            _index += source.Length;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            using (var e = source.GetEnumerator())
            {
                var i = _index;
                for (; i < Result.Length; ++i)
                {
                    if (!e.MoveNext())
                        break;

                    Result[i] = e.Current;
                }
                if (i == Result.Length && e.MoveNext())
                    throw new Exception("Logic error, enumerable contains more data that expected");
                _index = i;
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            if (_index + source.Length > Result.Length)
                throw new Exception("Logic error, enumerable contains more data that expected");

            if (_index == 0)
            {
                ToArrayHelper.Select(source, selector, Result);
            }
            else
            {
                ToArrayHelper.SelectNonZeroIndex(source, selector, Result, _index);
            }
            _index += source.Length;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            var index = _index;
            foreach (var item in source)
            {
                Result[index++] = selector(item);
            }
            _index = index;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var index = _index;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[index++] = item;
                }
            }
            _index = index;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            var index = _index;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[index++] = item;
                }
            }
            _index = index;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            var index = _index;
            foreach (var item in span)
            {
                Result[index++] = resultSelector(source, item);
            }
            _index = index;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = selector(item);
                }
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    Result[_index++] = selector(item);
                }
            }
            return ChainStatus.Flow;
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

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            Result = builder.ToArray();

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            foreach (var item in source)
            {
                builder.Add(item);
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
            {
                builder.Add(item);
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                builder.Add(selector(item));
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                builder.Add(selector(item));
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(item);
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var item in span)
            {
                builder.Add(resultSelector(source, item));
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    builder.Add(selector(item));
                }
            }
            return ChainStatus.Flow;
        }
    }
}
