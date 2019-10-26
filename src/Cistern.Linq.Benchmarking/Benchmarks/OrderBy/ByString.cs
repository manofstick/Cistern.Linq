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
    |      Method | PreSorted | CustomerCount |            Mean |          Error |          StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |----------------:|---------------:|----------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |        101.7 ns |       1.367 ns |       1.2117 ns |  1.00 |    0.00 |   0.0458 |       - |       - |     144 B |
    | CisternLinq |    Random |             0 |        279.5 ns |       4.501 ns |       3.9903 ns |  2.75 |    0.05 |   0.0682 |       - |       - |     216 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |             1 |        199.1 ns |       1.132 ns |       1.0587 ns |  1.00 |    0.00 |   0.1402 |       - |       - |     440 B |
    | CisternLinq |    Random |             1 |        507.7 ns |       6.988 ns |       6.5362 ns |  2.55 |    0.04 |   0.1545 |       - |       - |     488 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |            10 |      2,390.4 ns |      32.002 ns |      29.9348 ns |  1.00 |    0.00 |   0.2899 |       - |       - |     920 B |
    | CisternLinq |    Random |            10 |      3,244.0 ns |      48.095 ns |      44.9881 ns |  1.36 |    0.03 |   0.2861 |       - |       - |     912 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |           100 |     58,667.2 ns |     301.361 ns |     281.8928 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4584 B |
    | CisternLinq |    Random |           100 |     61,082.7 ns |     898.245 ns |     840.2189 ns |  1.04 |    0.01 |   1.3428 |       - |       - |    4400 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |          1000 |    899,987.6 ns |  10,323.099 ns |   9,656.2328 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36992 B |
    | CisternLinq |    Random |          1000 |    917,157.6 ns |   4,400.974 ns |   4,116.6737 ns |  1.02 |    0.01 |  11.7188 |       - |       - |   36928 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |         10000 | 12,796,677.2 ns | 134,784.193 ns | 119,482.6764 ns |  1.00 |    0.00 |  93.7500 | 31.2500 | 31.2500 |  331752 B |
    | CisternLinq |    Random |         10000 | 12,451,425.3 ns |  89,944.053 ns |  84,133.7228 ns |  0.97 |    0.01 |  93.7500 | 31.2500 |       - |  386520 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             0 |        101.2 ns |       1.059 ns |       0.9906 ns |  1.00 |    0.00 |   0.0459 |       - |       - |     144 B |
    | CisternLinq |   Forward |             0 |        269.1 ns |       3.323 ns |       3.1086 ns |  2.66 |    0.04 |   0.0687 |       - |       - |     216 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             1 |        194.6 ns |       2.575 ns |       2.4087 ns |  1.00 |    0.00 |   0.1400 |       - |       - |     440 B |
    | CisternLinq |   Forward |             1 |        527.0 ns |       4.291 ns |       3.3503 ns |  2.72 |    0.02 |   0.1554 |       - |       - |     488 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |            10 |      1,194.9 ns |      17.570 ns |      16.4350 ns |  1.00 |    0.00 |   0.2918 |       - |       - |     920 B |
    | CisternLinq |   Forward |            10 |      2,414.8 ns |      31.587 ns |      29.5462 ns |  2.02 |    0.03 |   0.2861 |       - |       - |     912 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |           100 |     32,356.7 ns |     576.168 ns |     538.9477 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4584 B |
    | CisternLinq |   Forward |           100 |     40,420.7 ns |     594.472 ns |     556.0698 ns |  1.25 |    0.03 |   1.3428 |       - |       - |    4400 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |          1000 |    554,975.6 ns |   7,498.069 ns |   7,013.6985 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36992 B |
    | CisternLinq |   Forward |          1000 |    598,707.0 ns |   7,488.092 ns |   7,004.3655 ns |  1.08 |    0.02 |  11.7188 |       - |       - |   36928 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |         10000 |  8,773,193.5 ns | 123,216.912 ns | 115,257.1755 ns |  1.00 |    0.00 | 109.3750 | 31.2500 | 31.2500 |  331752 B |
    | CisternLinq |   Forward |         10000 |  9,129,773.3 ns | 165,026.059 ns | 154,365.4778 ns |  1.04 |    0.02 |  93.7500 | 31.2500 |       - |  386520 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             0 |        105.2 ns |       1.779 ns |       1.6644 ns |  1.00 |    0.00 |   0.0459 |       - |       - |     144 B |
    | CisternLinq |   Reverse |             0 |        276.8 ns |       5.052 ns |       4.2186 ns |  2.62 |    0.05 |   0.0682 |       - |       - |     216 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             1 |        192.6 ns |       2.245 ns |       2.0995 ns |  1.00 |    0.00 |   0.1400 |       - |       - |     440 B |
    | CisternLinq |   Reverse |             1 |        515.5 ns |       7.907 ns |       7.0092 ns |  2.68 |    0.05 |   0.1554 |       - |       - |     488 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |            10 |      4,079.4 ns |      64.302 ns |      60.1479 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     920 B |
    | CisternLinq |   Reverse |            10 |      5,003.1 ns |      59.706 ns |      55.8493 ns |  1.23 |    0.03 |   0.2823 |       - |       - |     912 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |           100 |     45,722.0 ns |     408.893 ns |     382.4790 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4584 B |
    | CisternLinq |   Reverse |           100 |     53,511.5 ns |     852.120 ns |     797.0737 ns |  1.17 |    0.02 |   1.3428 |       - |       - |    4400 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |          1000 |    870,802.6 ns |   9,127.610 ns |   8,537.9721 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36992 B |
    | CisternLinq |   Reverse |          1000 |    865,110.9 ns |  12,637.834 ns |  11,821.4374 ns |  0.99 |    0.01 |  11.7188 |       - |       - |   36928 B |
    |             |           |               |                 |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 14,565,501.2 ns | 236,854.797 ns | 221,554.1244 ns |  1.00 |    0.00 |  93.7500 | 31.2500 | 31.2500 |  331752 B |
    | CisternLinq |   Reverse |         10000 | 13,774,254.5 ns | 107,964.149 ns | 100,989.7322 ns |  0.95 |    0.02 |  93.7500 | 15.6250 |       - |  386520 B |
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
