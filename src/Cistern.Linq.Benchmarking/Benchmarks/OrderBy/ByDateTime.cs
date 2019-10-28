using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    /*
    |      Method | PreSorted | CustomerCount |            Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |----------------:|--------------:|--------------:|------:|--------:|--------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |        70.69 ns |      1.437 ns |      1.274 ns |  1.00 |    0.00 |  0.0356 |       - |       - |     112 B |
    | CisternLinq |    Random |             0 |       138.76 ns |      1.599 ns |      1.496 ns |  1.97 |    0.03 |  0.0355 |       - |       - |     112 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |    Random |             1 |       170.01 ns |      2.941 ns |      2.751 ns |  1.00 |    0.00 |  0.1299 |       - |       - |     408 B |
    | CisternLinq |    Random |             1 |       329.28 ns |      3.088 ns |      2.889 ns |  1.94 |    0.04 |  0.1140 |       - |       - |     360 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |    Random |            10 |       683.43 ns |      7.359 ns |      6.884 ns |  1.00 |    0.00 |  0.2823 |       - |       - |     888 B |
    | CisternLinq |    Random |            10 |       718.61 ns |      6.591 ns |      6.165 ns |  1.05 |    0.02 |  0.1926 |       - |       - |     608 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |    Random |           100 |     8,249.43 ns |    107.306 ns |    100.374 ns |  1.00 |    0.00 |  1.4496 |       - |       - |    4552 B |
    | CisternLinq |    Random |           100 |     4,766.43 ns |     67.150 ns |     62.812 ns |  0.58 |    0.01 |  1.2665 |       - |       - |    3984 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |    Random |          1000 |   138,668.52 ns |  1,922.783 ns |  1,798.572 ns |  1.00 |    0.00 | 11.7188 |       - |       - |   36960 B |
    | CisternLinq |    Random |          1000 |    68,357.98 ns |    654.580 ns |    612.295 ns |  0.49 |    0.01 | 12.4512 |       - |       - |   39216 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |    Random |         10000 | 2,001,044.43 ns | 38,804.533 ns | 38,111.240 ns |  1.00 |    0.00 | 97.6563 | 42.9688 | 39.0625 |  331720 B |
    | CisternLinq |    Random |         10000 | 1,116,043.48 ns | 11,272.311 ns |  9,992.610 ns |  0.56 |    0.01 | 97.6563 | 27.3438 |       - |  381120 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |             0 |        71.77 ns |      1.326 ns |      1.241 ns |  1.00 |    0.00 |  0.0356 |       - |       - |     112 B |
    | CisternLinq |   Forward |             0 |       139.97 ns |      1.299 ns |      1.215 ns |  1.95 |    0.04 |  0.0355 |       - |       - |     112 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |             1 |       169.39 ns |      2.056 ns |      1.923 ns |  1.00 |    0.00 |  0.1297 |       - |       - |     408 B |
    | CisternLinq |   Forward |             1 |       324.07 ns |      3.569 ns |      3.338 ns |  1.91 |    0.02 |  0.1144 |       - |       - |     360 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |            10 |       537.43 ns |      7.265 ns |      6.796 ns |  1.00 |    0.00 |  0.2823 |       - |       - |     888 B |
    | CisternLinq |   Forward |            10 |       670.57 ns |      8.328 ns |      7.790 ns |  1.25 |    0.02 |  0.1936 |       - |       - |     608 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |           100 |     5,476.48 ns |     53.616 ns |     47.529 ns |  1.00 |    0.00 |  1.4496 |       - |       - |    4552 B |
    | CisternLinq |   Forward |           100 |     4,311.67 ns |     45.570 ns |     42.626 ns |  0.79 |    0.01 |  1.2665 |       - |       - |    3984 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |          1000 |    72,713.20 ns |    972.693 ns |    909.858 ns |  1.00 |    0.00 | 11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Forward |          1000 |    44,616.09 ns |    297.549 ns |    278.327 ns |  0.61 |    0.01 | 12.4512 |       - |       - |   39216 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Forward |         10000 | 1,199,387.65 ns | 23,894.451 ns | 31,069.535 ns |  1.00 |    0.00 | 89.8438 | 44.9219 | 41.0156 |  331720 B |
    | CisternLinq |   Forward |         10000 |   608,142.44 ns |  2,927.363 ns |  2,595.031 ns |  0.51 |    0.01 | 98.6328 | 28.3203 |       - |  381120 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |             0 |        72.73 ns |      1.314 ns |      1.229 ns |  1.00 |    0.00 |  0.0356 |       - |       - |     112 B |
    | CisternLinq |   Reverse |             0 |       138.18 ns |      1.785 ns |      1.670 ns |  1.90 |    0.03 |  0.0355 |       - |       - |     112 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |             1 |       169.12 ns |      2.724 ns |      2.548 ns |  1.00 |    0.00 |  0.1297 |       - |       - |     408 B |
    | CisternLinq |   Reverse |             1 |       320.43 ns |      3.234 ns |      3.025 ns |  1.90 |    0.03 |  0.1144 |       - |       - |     360 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |            10 |       841.56 ns |     11.159 ns |     10.438 ns |  1.00 |    0.00 |  0.2823 |       - |       - |     888 B |
    | CisternLinq |   Reverse |            10 |       787.55 ns |      6.731 ns |      6.297 ns |  0.94 |    0.01 |  0.1926 |       - |       - |     608 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |           100 |     7,335.97 ns |    112.676 ns |    105.397 ns |  1.00 |    0.00 |  1.4496 |       - |       - |    4552 B |
    | CisternLinq |   Reverse |           100 |     4,699.70 ns |     49.955 ns |     44.284 ns |  0.64 |    0.01 |  1.2665 |       - |       - |    3984 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |          1000 |   105,504.05 ns |    780.462 ns |    651.721 ns |  1.00 |    0.00 | 11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Reverse |          1000 |    49,154.79 ns |    639.469 ns |    598.160 ns |  0.47 |    0.01 | 12.4512 |       - |       - |   39216 B |
    |             |           |               |                 |               |               |       |         |         |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 1,701,534.24 ns | 32,916.445 ns | 32,328.350 ns |  1.00 |    0.00 | 89.8438 | 44.9219 | 41.0156 |  331720 B |
    | CisternLinq |   Reverse |         10000 |   688,353.03 ns |  6,938.490 ns |  6,490.267 ns |  0.40 |    0.01 | 98.6328 | 28.3203 |       - |  381120 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class OrderBy_ByDateTime
        : OrderByBase
    {
        [Params(SortOrder.Random, SortOrder.Forward, SortOrder.Reverse)]
        public SortOrder PreSorted = SortOrder.Random;

        protected override void GlobalSetup()
        {
            _customers = PreSorted switch
            {
                SortOrder.Forward => _customers.OrderBy(x => x.DOB).ToArray(),
                SortOrder.Reverse => _customers.OrderByDescending(x => x.DOB).ToArray(),
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
                        c => c.DOB
                    )
                );
        }

        [Benchmark]
        public Cistern.Linq.Benchmarking.Data.DummyData.Customer[] CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.DOB)
                .ToArray();
        }

    }
}
