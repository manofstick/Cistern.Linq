using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    /*
    |      Method | PreSorted | CustomerCount |            Mean |          Error |         StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |----------------:|---------------:|---------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |        75.82 ns |      0.7996 ns |      0.7479 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |    Random |             0 |       235.65 ns |      4.1474 ns |      3.8795 ns |  3.11 |    0.05 |   0.0505 |       - |       - |     160 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |    Random |             1 |       172.29 ns |      1.7068 ns |      1.5966 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |    Random |             1 |       439.00 ns |      4.6239 ns |      4.3252 ns |  2.55 |    0.03 |   0.1473 |       - |       - |     464 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |    Random |            10 |       713.77 ns |      4.4486 ns |      4.1612 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |    Random |            10 |     1,001.63 ns |     12.3462 ns |     11.5487 ns |  1.40 |    0.02 |   0.2823 |       - |       - |     888 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |    Random |           100 |     8,538.07 ns |    157.4675 ns |    147.2952 ns |  1.00 |    0.00 |   1.4496 |       - |       - |    4552 B |
    | CisternLinq |    Random |           100 |     5,421.50 ns |     73.4778 ns |     61.3573 ns |  0.64 |    0.01 |   1.3885 |       - |       - |    4376 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |    Random |          1000 |   140,918.58 ns |  1,801.4794 ns |  1,685.1049 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |    Random |          1000 |    74,449.00 ns |  1,246.7721 ns |  1,166.2314 ns |  0.53 |    0.01 |  11.7188 |       - |       - |   36904 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |    Random |         10000 | 2,123,144.62 ns | 40,253.7915 ns | 43,071.1059 ns |  1.00 |    0.00 |  97.6563 | 46.8750 | 39.0625 |  331720 B |
    | CisternLinq |    Random |         10000 | 1,176,935.18 ns | 15,416.9078 ns | 14,420.9851 ns |  0.56 |    0.01 | 103.5156 | 33.2031 |       - |  386496 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             0 |        73.14 ns |      0.8307 ns |      0.7771 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Forward |             0 |       227.09 ns |      4.6347 ns |      4.7595 ns |  3.10 |    0.08 |   0.0508 |       - |       - |     160 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             1 |       178.57 ns |      1.6118 ns |      1.5077 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |   Forward |             1 |       461.33 ns |      7.5249 ns |      7.0388 ns |  2.58 |    0.04 |   0.1478 |       - |       - |     464 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |            10 |       542.08 ns |      7.7205 ns |      6.4470 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Forward |            10 |       939.52 ns |      7.4634 ns |      6.6161 ns |  1.73 |    0.03 |   0.2823 |       - |       - |     888 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |           100 |     5,729.97 ns |     78.6389 ns |     73.5589 ns |  1.00 |    0.00 |   1.4496 |       - |       - |    4552 B |
    | CisternLinq |   Forward |           100 |     5,042.42 ns |     21.3238 ns |     18.9030 ns |  0.88 |    0.01 |   1.3885 |       - |       - |    4376 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |          1000 |    75,733.74 ns |    295.7398 ns |    276.6352 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Forward |          1000 |    48,714.66 ns |    532.4954 ns |    472.0433 ns |  0.64 |    0.01 |  11.7188 |       - |       - |   36904 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Forward |         10000 | 1,180,685.10 ns | 23,235.6857 ns | 31,018.9731 ns |  1.00 |    0.00 |  89.8438 | 44.9219 | 41.0156 |  331720 B |
    | CisternLinq |   Forward |         10000 |   658,358.33 ns |  5,556.6041 ns |  5,197.6509 ns |  0.56 |    0.01 | 103.5156 | 34.1797 |       - |  386496 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             0 |        72.73 ns |      1.5471 ns |      1.5888 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Reverse |             0 |       228.85 ns |      1.6404 ns |      1.5345 ns |  3.14 |    0.07 |   0.0508 |       - |       - |     160 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             1 |       171.64 ns |      2.1928 ns |      2.0512 ns |  1.00 |    0.00 |   0.1299 |       - |       - |     408 B |
    | CisternLinq |   Reverse |             1 |       448.91 ns |      3.8416 ns |      3.5934 ns |  2.62 |    0.04 |   0.1473 |       - |       - |     464 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |            10 |       837.10 ns |      8.6764 ns |      8.1159 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Reverse |            10 |     1,072.60 ns |     14.3677 ns |     13.4396 ns |  1.28 |    0.02 |   0.2823 |       - |       - |     888 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |           100 |     7,243.94 ns |     81.4088 ns |     76.1499 ns |  1.00 |    0.00 |   1.4496 |       - |       - |    4552 B |
    | CisternLinq |   Reverse |           100 |     5,591.33 ns |     43.1524 ns |     40.3648 ns |  0.77 |    0.01 |   1.3885 |       - |       - |    4376 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |          1000 |   105,846.47 ns |  1,511.2144 ns |  1,413.5909 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Reverse |          1000 |    53,837.24 ns |    680.8774 ns |    636.8931 ns |  0.51 |    0.01 |  11.7188 |       - |       - |   36904 B |
    |             |           |               |                 |                |                |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 1,742,237.58 ns | 34,638.1961 ns | 39,889.3981 ns |  1.00 |    0.00 |  89.8438 | 44.9219 | 41.0156 |  331720 B |
    | CisternLinq |   Reverse |         10000 |   733,869.07 ns |  6,853.7577 ns |  6,411.0092 ns |  0.42 |    0.01 | 103.5156 | 34.1797 |       - |  386496 B |
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
