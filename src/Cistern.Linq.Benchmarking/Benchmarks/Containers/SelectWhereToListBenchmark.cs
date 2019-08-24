using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<string>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers
{
    [CoreJob, MemoryDiagnoser]
    public class SelectWhereToListBenchmark : ContainersBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var subset = new List<string>();
            foreach (var c in Customers)
            {
                if (c.DOB.Year < 1980)
                {
                    subset.Add(c.Name);
                }
            }
            return subset;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.Select(
                        System.Linq.Enumerable.Where(
                            Customers,
                            c => c.DOB.Year < 1980),
                        c => c.Name)
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return 
                Customers
                .Where(c => c.DOB.Year < 1980)
                .Select(c => c.Name)
                .ToList();
        }

    }
}
