using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    /*
    */
    [CoreJob, MemoryDiagnoser]
    public class OrderBy_ByInt
        : OrderByBase
    {
        [Params(SortOrder.Random, SortOrder.Forward, SortOrder.Reverse)]
        public SortOrder PreSorted = SortOrder.Random;

        protected override void GlobalSetup()
        {
            _customers = PreSorted switch
            {
                SortOrder.Forward => _customers.OrderBy(x => x.PostCode).ToArray(),
                SortOrder.Reverse => _customers.OrderByDescending(x => x.PostCode).ToArray(),
                _ => _customers
            };
        }

        [Benchmark(Baseline = true)]
        public Cistern.Linq.Benchmarking.Data.DummyData.Customer[] SystemLinq()
        {
            return
                System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.OrderBy(
                        Customers,
                        c => c.PostCode
                    )
                );
        }

        [Benchmark]
        public Cistern.Linq.Benchmarking.Data.DummyData.Customer[] CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.PostCode)
                .ToArray();
        }

    }
}
