using System;
using System.Collections.Generic;

namespace Cistern.Linq.Consumer
{

    sealed class ToDictionary<TSource, TKey, TElement>
        : Consumer<TSource, Dictionary<TKey, TElement>>
        , Optimizations.IHeadStart<TSource>
        , Optimizations.ITailEnd<TSource>
    {
        Func<TSource, TKey> _keySelector;
        Func<TSource, TElement> _elementSelector;

        public ToDictionary(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(comparer)) => (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public ToDictionary(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(capacity, comparer)) => (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public override ChainStatus ProcessNext(TSource input)
        {
            Result.Add(_keySelector(input), _elementSelector(input));

            return ChainStatus.Flow;
        }
        private void EnsureCapacity(int length)
        {
            Result.EnsureCapacity(Result.Count + length);
        }

        private void EnsureCapacity(int? maybeLength)
        {
            if (maybeLength.HasValue)
            {
                Result.EnsureCapacity(Result.Count + maybeLength.Value);
            }
        }

        ChainStatus Optimizations.IHeadStart<TSource>.Execute(ReadOnlySpan<TSource> source)
        {
            
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var item in source)
            {
                result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            
            var result = Result;

            EnsureCapacity(source.TryLength);
            foreach (var item in source)
            {
                result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.Select<S>(ReadOnlySpan<S> source, Func<S, TSource> selector)
        {
            
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                var item = selector(input);
                result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.Select<Enumerable, Enumerator, S>(Enumerable source, Func<S, TSource> selector)
        {
            
            var result = Result;

            var maybeLength = source.TryLength;
            if (maybeLength.HasValue)
                EnsureCapacity(maybeLength.Value);

            foreach (var input in source)
            {
                var item = selector(input);
                result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.SelectMany<TSource1, TCollection>(TSource1 source, ReadOnlySpan<TCollection> span, Func<TSource1, TCollection, TSource> resultSelector)
        {
            
            var result = Result;

            EnsureCapacity(span.Length);
            foreach (var input in span)
            {
                var item = resultSelector(source, input);
                result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.Where(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
        {
            
            var result = Result;

            foreach (var item in source)
            {
                if (predicate(item))
                    result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.Where<Enumerable, Enumerator>(Enumerable source, Func<TSource, bool> predicate)
        {
            
            var result = Result;

            foreach (var item in source)
            {
                if (predicate(item))
                    result.Add(_keySelector(item), _elementSelector(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, TSource> selector)
        {
            
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var item = selector(input);
                    result.Add(_keySelector(item), _elementSelector(item));
                }
            }

            Result = result;
            return ChainStatus.Flow;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, TSource> selector)
        {
            
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var item = selector(input);
                    result.Add(_keySelector(item), _elementSelector(item));
                }
            }

            Result = result;
            return ChainStatus.Flow;
        }
    }
}
