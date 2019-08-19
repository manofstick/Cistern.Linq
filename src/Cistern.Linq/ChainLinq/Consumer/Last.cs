using System;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class Last<T>
        : Consumer<T, T>
        , Optimizations.ITailEnd<T>
    {
        private bool _found;
        private bool _orDefault;

        public Last(bool orDefault) : base(default(T)) =>
            (_orDefault, _found) = (orDefault, false);

        public override ChainStatus ProcessNext(T input)
        {
            _found = true;
            Result = input;
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (!_orDefault && !_found)
            {
                ThrowHelper.ThrowNoElementsException();
            }
        }

        void Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            // TODO: Could optimize, if we assumed selector was immutable
            foreach (var input in source)
            {
                _found = true;
                Result = selector(input);
            }
        }

        void Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            // assuming predicate is pure; reverse search was a System.Linq optimization
            for(var i=source.Length-1; i >= 0; --i)
            {
                var input = source[i];
                if (predicate(input))
                {
                    _found = true;
                    Result = input;
                    return;
                }
            }
        }

        void Optimizations.ITailEnd<T>.Where<Enumerator>(Optimizations.ITypedEnumerable<T, Enumerator> source, Func<T, bool> predicate)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    _found = true;
                    Result = input;
                }
            }
        }

        ChainStatus Optimizations.ITailEnd<T>.SelectMany<TSource, TCollection>(TSource source, ReadOnlySpan<TCollection> span, Func<TSource, TCollection, T> resultSelector)
        {
            foreach (var input in span)
            {
                _found = true;
                Result = resultSelector(source, input);
            }
            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    _found = true;
                    Result = selector(input);
                }
            }
        }
    }
}
