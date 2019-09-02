using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    interface ISelectors<TSource, TKey, TElement>
    {
        TKey Key(TSource source);
        TElement Element(TSource source);
    }

    struct KeySourceSelector<TSource, TKey>
        : ISelectors<TSource, TKey, TSource>
    {
        Func<TSource, TKey> keySelector;

        public KeySourceSelector(Func<TSource, TKey> keySelector) => this.keySelector = keySelector;

        public TSource Element(TSource source) => source;
        public TKey Key(TSource source) => keySelector(source);
    }

    struct KeyElementSelector<TSource, TKey, TElement>
        : ISelectors<TSource, TKey, TElement>
    {
        Func<TSource, TKey> keySelector;
        Func<TSource, TElement> elementSelector;

        public KeyElementSelector(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
        }

        public TElement Element(TSource source) => elementSelector(source);
        public TKey Key(TSource source) => keySelector(source);
    }

    sealed class ToDictionary<Selector, TSource, TKey, TElement>
        : Consumer<TSource, Dictionary<TKey, TElement>>
        , Optimizations.IHeadStart<TSource>
        , Optimizations.ITailEnd<TSource>
        where Selector : ISelectors<TSource, TKey, TElement>
    {
        Selector _selector;

        public ToDictionary(Selector selector, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(comparer)) => _selector = selector;

        public ToDictionary(Selector selector, int capacity, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(capacity, comparer)) => _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result.Add(_selector.Key(input), _selector.Element(input));

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

        void Optimizations.IHeadStart<TSource>.Execute(ReadOnlySpan<TSource> source)
        {
            var s = _selector;
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var item in source)
            {
                result.Add(s.Key(item), s.Element(item));
            }

            Result = result;
        }

        void Optimizations.IHeadStart<TSource>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            var s = _selector;
            var result = Result;

            EnsureCapacity(source.TryLength);
            foreach (var item in source)
            {
                result.Add(s.Key(item), s.Element(item));
            }

            Result = result;
        }

        void Optimizations.ITailEnd<TSource>.Select<S>(ReadOnlySpan<S> source, Func<S, TSource> selector)
        {
            var s = _selector;
            var result = Result;

            EnsureCapacity(source.Length);
            foreach (var input in source)
            {
                var item = selector(input);
                result.Add(s.Key(item), s.Element(item));
            }

            Result = result;
        }

        ChainStatus Optimizations.ITailEnd<TSource>.SelectMany<TSource1, TCollection>(TSource1 source, ReadOnlySpan<TCollection> span, Func<TSource1, TCollection, TSource> resultSelector)
        {
            var s = _selector;
            var result = Result;

            EnsureCapacity(span.Length);
            foreach (var input in span)
            {
                var item = resultSelector(source, input);
                result.Add(s.Key(item), s.Element(item));
            }

            Result = result;

            return ChainStatus.Flow;
        }

        void Optimizations.ITailEnd<TSource>.Where(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
        {
            var s = _selector;
            var result = Result;

            foreach (var item in source)
            {
                if (predicate(item))
                    result.Add(s.Key(item), s.Element(item));
            }

            Result = result;
        }

        void Optimizations.ITailEnd<TSource>.Where<Enumerable, Enumerator>(Enumerable source, Func<TSource, bool> predicate)
        {
            var s = _selector;
            var result = Result;

            foreach (var item in source)
            {
                if (predicate(item))
                    result.Add(s.Key(item), s.Element(item));
            }

            Result = result;
        }

        void Optimizations.ITailEnd<TSource>.WhereSelect<S>(ReadOnlySpan<S> source, Func<S, bool> predicate, Func<S, TSource> selector)
        {
            var s = _selector;
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var item = selector(input);
                    result.Add(s.Key(item), s.Element(item));
                }
            }

            Result = result;
        }

        void Optimizations.ITailEnd<TSource>.WhereSelect<Enumerable, Enumerator, S>(Enumerable source, Func<S, bool> predicate, Func<S, TSource> selector)
        {
            var s = _selector;
            var result = Result;

            foreach (var input in source)
            {
                if (predicate(input))
                {
                    var item = selector(input);
                    result.Add(s.Key(item), s.Element(item));
                }
            }

            Result = result;
        }
    }
}
