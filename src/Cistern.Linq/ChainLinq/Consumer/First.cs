using System;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class First<T>
        : Consumer<T, T>
        , Optimizations.IHeadStart<T>
        , Optimizations.ITailEnd<T>
    {
        private bool _found;
        private bool _orDefault;

        public First(bool orDefault) : base(default) =>
            (_orDefault, _found) = (orDefault, false);

        public override ChainStatus ProcessNext(T input)
        {
            _found = true;
            Result = input;
            return ChainStatus.Stop;
        }

        public override void ChainComplete()
        {
            if (!_orDefault && !_found)
            {
                ThrowHelper.ThrowNoElementsException();
            }
        }

        void Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            foreach(var item in source)
            {
                _found = true;
                Result = item;
                return;
            }
        }

        void Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            foreach (var item in source)
            {
                _found = true;
                Result = item;
                return;
            }
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                _found = true;
                Result = selector(item);
                return;
            }
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    _found = true;
                    Result = item;
                    return;
                }
            }
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    _found = true;
                    Result = item;
                    return;
                }
            }
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var item in span)
            {
                _found = true;
                Result = resultSelector(source, item);
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    _found = true;
                    Result = selector(item);
                    return;
                }
            }
        }

        void Optimizations.ITailEnd<T>.WhereSelect<Enumerator, S>(Optimizations.ITypedEnumerable<S, Enumerator> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    _found = true;
                    Result = selector(item);
                    return;
                }
            }
        }
    }
}
