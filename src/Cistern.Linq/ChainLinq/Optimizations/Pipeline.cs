using System;
using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IHeadStart<T>
    {
        void Execute(ReadOnlySpan<T> source);
        void Execute(List<T> source);
        void Execute(IList<T> source, int start, int length);
        void Execute(IEnumerable<T> source);
    }
}
