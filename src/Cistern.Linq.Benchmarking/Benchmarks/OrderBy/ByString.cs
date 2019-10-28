using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    public enum SortOrder
    {
        Random,
        Forward,
        Reverse
    }

    /*
    |      Method | PreSorted | CustomerCount |             Mean |           Error |          StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |-----------------:|----------------:|----------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |         80.31 ns |       1.5936 ns |       1.5651 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |    Random |             0 |        150.44 ns |       3.0432 ns |       3.3825 ns |  1.87 |    0.06 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |             1 |        186.39 ns |       3.2186 ns |       3.0107 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |    Random |             1 |        358.69 ns |       3.6759 ns |       3.4385 ns |  1.93 |    0.04 |   0.1144 |       - |       - |     360 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |            10 |      2,260.30 ns |      27.5014 ns |      25.7249 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |    Random |            10 |      2,280.17 ns |      30.5511 ns |      28.5775 ns |  1.01 |    0.02 |   0.2098 |       - |       - |     672 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |           100 |     58,857.99 ns |     553.4962 ns |     517.7407 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |    Random |           100 |     55,941.04 ns |     714.4884 ns |     668.3329 ns |  0.95 |    0.01 |   1.2817 |       - |       - |    4048 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |          1000 |    902,605.51 ns |   4,109.9598 ns |   3,208.7863 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |    Random |          1000 |    900,122.67 ns |  10,081.3161 ns |   9,430.0693 ns |  1.00 |    0.01 |  11.7188 |       - |       - |   39216 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |         10000 | 12,676,316.88 ns | 167,583.1290 ns | 156,757.3629 ns |  1.00 |    0.00 | 109.3750 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |    Random |         10000 | 12,201,408.75 ns | 146,119.9630 ns | 136,680.7040 ns |  0.96 |    0.02 |  93.7500 | 31.2500 |       - |  381120 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             0 |         80.85 ns |       1.5522 ns |       1.4519 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Forward |             0 |        143.56 ns |       1.6379 ns |       1.5321 ns |  1.78 |    0.04 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             1 |        181.95 ns |       1.7910 ns |       1.6753 ns |  1.00 |    0.00 |   0.1299 |       - |       - |     408 B |
    | CisternLinq |   Forward |             1 |        340.96 ns |       4.8689 ns |       4.5544 ns |  1.87 |    0.03 |   0.1144 |       - |       - |     360 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |            10 |      1,203.79 ns |      17.0664 ns |      15.1289 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Forward |            10 |      1,394.29 ns |      12.9508 ns |      12.1142 ns |  1.16 |    0.02 |   0.2117 |       - |       - |     672 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |           100 |     33,693.26 ns |     547.5114 ns |     512.1425 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |   Forward |           100 |     32,577.55 ns |     466.9113 ns |     436.7492 ns |  0.97 |    0.01 |   1.2817 |       - |       - |    4048 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |          1000 |    553,685.42 ns |   7,959.8430 ns |   7,445.6421 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Forward |          1000 |    576,387.82 ns |   7,079.0310 ns |   6,621.7300 ns |  1.04 |    0.02 |  11.7188 |       - |       - |   39216 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |         10000 |  9,312,735.05 ns | 119,474.2765 ns | 111,756.3124 ns |  1.00 |    0.00 | 109.3750 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |   Forward |         10000 |  9,429,321.25 ns | 103,613.2673 ns |  96,919.9145 ns |  1.01 |    0.01 |  93.7500 | 15.6250 |       - |  381120 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             0 |         82.38 ns |       0.6635 ns |       0.6207 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Reverse |             0 |        148.07 ns |       1.7552 ns |       1.4656 ns |  1.80 |    0.02 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             1 |        186.38 ns |       2.2859 ns |       2.1382 ns |  1.00 |    0.00 |   0.1299 |       - |       - |     408 B |
    | CisternLinq |   Reverse |             1 |        340.15 ns |       2.6465 ns |       2.4756 ns |  1.83 |    0.03 |   0.1144 |       - |       - |     360 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |            10 |      3,773.50 ns |      52.2326 ns |      48.8584 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Reverse |            10 |      4,078.52 ns |      42.3550 ns |      37.5466 ns |  1.08 |    0.02 |   0.2136 |       - |       - |     672 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |           100 |     46,195.02 ns |     596.9095 ns |     558.3495 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |   Reverse |           100 |     46,933.62 ns |   1,003.8102 ns |   1,194.9646 ns |  1.02 |    0.02 |   1.2817 |       - |       - |    4048 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |          1000 |    911,591.39 ns |  13,006.1395 ns |  12,165.9510 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Reverse |          1000 |    875,882.95 ns |   9,845.9029 ns |   9,209.8637 ns |  0.96 |    0.02 |  11.7188 |       - |       - |   39216 B |
    |             |           |               |                  |                 |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 14,963,840.89 ns | 229,528.3379 ns | 214,700.9496 ns |  1.00 |    0.00 |  93.7500 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |   Reverse |         10000 | 14,217,630.47 ns | 246,869.9064 ns | 218,843.7419 ns |  0.95 |    0.02 |  93.7500 | 31.2500 |       - |  381120 B | 
    */
    [CoreJob, MemoryDiagnoser]
    public class OrderBy_ByString
        : OrderByBase
    {
        [Params(SortOrder.Random, SortOrder.Forward, SortOrder.Reverse)]
        public SortOrder PreSorted = SortOrder.Random;

        protected override void GlobalSetup()
        {
            _customers = PreSorted switch
            {
                SortOrder.Forward => _customers.OrderBy(x => x.Name).ToArray(),
                SortOrder.Reverse => _customers.OrderByDescending(x => x.Name).ToArray(),
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
                        c => c.Name
                    )
                );
        }

        [Benchmark]
        public Cistern.Linq.Benchmarking.Data.DummyData.Customer[] CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.Name)
                .ToArray();
        }

    }
}
