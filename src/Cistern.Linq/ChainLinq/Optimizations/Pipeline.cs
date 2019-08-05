using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Optimizations
{
    interface IPipeline<T>
    {
        void Pipeline(T source);
    }
}
