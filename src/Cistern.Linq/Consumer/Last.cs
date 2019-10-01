using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{
    sealed class Last<T>
        : Consumer<T, T>
        , Optimizations.IHeadStart<T>
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

        public override ChainStatus ChainComplete(ChainStatus status)
        {
            if (!_orDefault && !_found)
            {
                ThrowHelper.ThrowNoElementsException();
            }

            return ChainStatus.Stop;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            if (source.Length > 0)
            {
                _found = true;
                Result = source[source.Length-1];
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            if (source.TryLast(out var result))
            {
                _found = true;
                Result = result;
            }
            else
            {
                foreach (var input in source)
                {
                    _found = true;
                    Result = input;
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<S>(ReadOnlySpan<S> source, Func<S, T> selector)
        {
            // TODO: Could optimize, if we assumed selector was immutable
            foreach (var input in source)
            {
                _found = true;
                Result = selector(input);
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, T> selector)
        {
            // TODO: Could optimize, if we assumed selector was immutable
            foreach (var input in source)
            {
                _found = true;
                Result = selector(input);
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where(ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            // assuming predicate is pure; reverse search was a System.Linq optimization
            for(var i=source.Length-1; i >= 0; --i)
            {
                var input = source[i];
                if (predicate(input))
                {
                    _found = true;
                    Result = input;
                    break;
                }
            }

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.Where<Enumerable, Enumerator>(Enumerable source, Func<T, bool> predicate)
        {
            if (source.Source is System.Collections.Generic.IList<T> list)
            {
                // assuming predicate is pure; reverse search was a System.Linq optimization
                for (var i = list.Count - 1; i >= 0; --i)
                {
                    var input = list[i];
                    if (predicate(input))
                    {
                        _found = true;
                        Result = input;
                        break;
                    }
                }
            }
            else
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
            return ChainStatus.Flow;
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

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    _found = true;
                    Result = selector(input);
                }
            }
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<T>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, T> selector)
        {
            foreach (var input in source)
            {
                if (predicate(input))
                {
                    _found = true;
                    Result = selector(input);
                }
            }
            return ChainStatus.Flow;
        }
    }
}
