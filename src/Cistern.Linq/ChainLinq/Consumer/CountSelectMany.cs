using System.Collections.Generic;

namespace Cistern.Linq.ChainLinq.Consumer
{
    sealed class CountSelectMany<Enumerable, T> : Consumer<Enumerable, int?>
        where Enumerable : IEnumerable<T>
    {
        private readonly bool asConsumer;

        public CountSelectMany(bool asConsumer) : base(0) => this.asConsumer = asConsumer;

        public override ChainStatus ProcessNext(Enumerable input)
        {
            checked
            {
                Result += 
                    input switch
                    {
                        ICollection<T>                      x => x.Count,
                        Optimizations.IConsumableFastCount  x => x.TryFastCount(asConsumer),
                        System.Collections.ICollection      x => x.Count,
                        _                                     => null,
                    };

                if (!Result.HasValue)
                    return ChainStatus.Stop;

                return ChainStatus.Flow;
            }
        }
    }
}
