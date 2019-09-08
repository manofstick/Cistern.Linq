using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class CountSelectMany<Enumerable, T> : Consumer<Enumerable, int>
        where Enumerable : IEnumerable<T>
    {
        public CountSelectMany() : base(0) { }

        public override ChainStatus ProcessNext(Enumerable input)
        {
            checked
            {
                Result += input.Count();
            }
            return ChainStatus.Flow;
        }
    }
}
