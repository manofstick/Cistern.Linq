﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consume
{
    static class Range
    {
        struct RangeEnumerator : IEnumerator<int>
        {
            private readonly int End;

            public RangeEnumerator(int start, int count)
            {
                Current = unchecked(start - 1);
                End = unchecked(start + count - 1);
            }

            public int Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (Current == End)
                    return false;

                Current++;
                return true;
            }

            public void Reset() => throw new System.NotImplementedException();
        }

        struct RangeEnumerable
            : Optimizations.ITypedEnumerable<int, RangeEnumerator>
        {
            private readonly int Count;
            private readonly int Start;

            public RangeEnumerable(int start, int count)
            {
                Start = start;
                Count = count;
            }

            public IEnumerable<int> Source => null;

            public int? TryLength => Count;

            public RangeEnumerator GetEnumerator() => new RangeEnumerator(Start, Count);

            public bool TryGetSourceAsSpan(out ReadOnlySpan<int> readOnlySpan)
            {
                readOnlySpan = default;
                return false;
            }
        }

        public static void Invoke(int start, int count, Chain<int> chain)
        {
            try
            {
                if (chain is Optimizations.IHeadStart<int> optimized)
                {
                    optimized.Execute<RangeEnumerable, RangeEnumerator>(new RangeEnumerable(start, count));
                }
                else
                {
                    Pipeline(start, count, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<V>(int start, int count, Link<int, V> composition, Chain<V> consumer) =>
            Invoke(start, count, composition.Compose(consumer));

        private static void Pipeline(int start, int count, Chain<int> chain)
        {
            var current = unchecked(start - 1);
            var end = unchecked(start + count);
            while (unchecked(++current) != end)
            {
                var state = chain.ProcessNext(current);
                if (state.IsStopped())
                    break;
            }
        }

    }
}
