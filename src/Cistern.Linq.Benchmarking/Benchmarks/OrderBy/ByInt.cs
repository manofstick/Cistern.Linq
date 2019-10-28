using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks.OrderBy
{
    /*
    |      Method | PreSorted | CustomerCount |            Mean |          Error |         StdDev |          Median | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |        70.73 ns |      0.8098 ns |      0.7178 ns |        70.91 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |    Random |             0 |       149.82 ns |      2.1812 ns |      2.0403 ns |       149.87 ns |  2.12 |    0.03 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |             1 |       169.29 ns |      1.3600 ns |      1.2056 ns |       169.65 ns |  1.00 |    0.00 |   0.1299 |       - |       - |     408 B |
    | CisternLinq |    Random |             1 |       328.10 ns |      2.6004 ns |      2.4325 ns |       327.89 ns |  1.94 |    0.02 |   0.1140 |       - |       - |     360 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |            10 |       628.54 ns |      6.9700 ns |      6.1787 ns |       629.06 ns |  1.00 |    0.00 |   0.2699 |       - |       - |     848 B |
    | CisternLinq |    Random |            10 |       710.72 ns |      9.5940 ns |      8.9742 ns |       710.02 ns |  1.13 |    0.02 |   0.1802 |       - |       - |     568 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |           100 |     8,282.31 ns |    103.8241 ns |     97.1171 ns |     8,344.35 ns |  1.00 |    0.00 |   1.3123 |       - |       - |    4152 B |
    | CisternLinq |    Random |           100 |     4,603.10 ns |     51.8800 ns |     48.5286 ns |     4,583.93 ns |  0.56 |    0.01 |   1.1368 |       - |       - |    3584 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |          1000 |   137,545.70 ns |  1,288.9590 ns |  1,205.6930 ns |   137,901.90 ns |  1.00 |    0.00 |  10.4980 |       - |       - |   32960 B |
    | CisternLinq |    Random |          1000 |    74,346.10 ns |  1,088.0980 ns |  1,017.8075 ns |    74,037.93 ns |  0.54 |    0.01 |  11.1084 |       - |       - |   35216 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |    Random |         10000 | 2,151,606.16 ns | 42,849.4067 ns | 78,352.5630 ns | 2,185,037.89 ns |  1.00 |    0.00 | 117.1875 | 74.2188 | 39.0625 |  291720 B |
    | CisternLinq |    Random |         10000 | 1,053,350.47 ns | 14,295.2156 ns | 13,371.7536 ns | 1,058,619.53 ns |  0.50 |    0.02 |  93.7500 | 31.2500 |       - |  341120 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             0 |        72.98 ns |      0.5914 ns |      0.5532 ns |        73.07 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Forward |             0 |       137.51 ns |      1.8622 ns |      1.7419 ns |       137.42 ns |  1.88 |    0.04 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             1 |       171.94 ns |      2.5428 ns |      2.3786 ns |       172.37 ns |  1.00 |    0.00 |   0.1299 |       - |       - |     408 B |
    | CisternLinq |   Forward |             1 |       328.45 ns |      4.0803 ns |      3.8168 ns |       328.77 ns |  1.91 |    0.04 |   0.1144 |       - |       - |     360 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |            10 |       530.22 ns |      6.3477 ns |      5.9377 ns |       532.30 ns |  1.00 |    0.00 |   0.2689 |       - |       - |     848 B |
    | CisternLinq |   Forward |            10 |       668.00 ns |      6.8910 ns |      6.4458 ns |       669.94 ns |  1.26 |    0.02 |   0.1802 |       - |       - |     568 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |           100 |     5,281.28 ns |     75.5349 ns |     70.6554 ns |     5,306.89 ns |  1.00 |    0.00 |   1.3199 |       - |       - |    4152 B |
    | CisternLinq |   Forward |           100 |     4,201.06 ns |     51.6373 ns |     48.3016 ns |     4,227.57 ns |  0.80 |    0.01 |   1.1368 |       - |       - |    3584 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |          1000 |    68,167.49 ns |    986.6475 ns |    922.9107 ns |    68,381.09 ns |  1.00 |    0.00 |  10.4980 |       - |       - |   32960 B |
    | CisternLinq |   Forward |          1000 |    39,744.30 ns |    413.7084 ns |    366.7417 ns |    39,872.58 ns |  0.58 |    0.01 |  11.1694 |       - |       - |   35216 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Forward |         10000 | 1,254,764.85 ns | 25,042.5675 ns | 61,899.0223 ns | 1,278,778.03 ns |  1.00 |    0.00 | 121.0938 | 78.1250 | 41.0156 |  291720 B |
    | CisternLinq |   Forward |         10000 |   581,169.82 ns |  8,249.8947 ns |  7,716.9567 ns |   584,605.47 ns |  0.49 |    0.04 |  95.7031 | 31.2500 |       - |  341120 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             0 |        77.37 ns |      1.7328 ns |      2.3133 ns |        76.87 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Reverse |             0 |       140.31 ns |      1.7083 ns |      1.5144 ns |       139.82 ns |  1.80 |    0.08 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             1 |       172.29 ns |      2.1882 ns |      2.0468 ns |       171.90 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |   Reverse |             1 |       334.79 ns |      3.4705 ns |      3.2463 ns |       335.08 ns |  1.94 |    0.03 |   0.1144 |       - |       - |     360 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |            10 |       781.12 ns |     11.7502 ns |     10.9911 ns |       779.84 ns |  1.00 |    0.00 |   0.2699 |       - |       - |     848 B |
    | CisternLinq |   Reverse |            10 |       795.42 ns |      6.2458 ns |      5.8423 ns |       796.60 ns |  1.02 |    0.02 |   0.1802 |       - |       - |     568 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |           100 |     6,744.53 ns |    106.0612 ns |     99.2097 ns |     6,716.68 ns |  1.00 |    0.00 |   1.3199 |       - |       - |    4152 B |
    | CisternLinq |   Reverse |           100 |     4,489.23 ns |     59.7270 ns |     55.8687 ns |     4,506.11 ns |  0.67 |    0.01 |   1.1368 |       - |       - |    3584 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |          1000 |   100,821.92 ns |  1,634.0500 ns |  1,448.5427 ns |   101,372.88 ns |  1.00 |    0.00 |  10.4980 |       - |       - |   32960 B |
    | CisternLinq |   Reverse |          1000 |    45,416.01 ns |    745.4918 ns |    697.3335 ns |    45,415.79 ns |  0.45 |    0.01 |  11.1694 |       - |       - |   35216 B |
    |             |           |               |                 |                |                |                 |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 1,593,504.02 ns | 85,656.9946 ns | 87,963.4270 ns | 1,550,664.26 ns |  1.00 |    0.00 | 121.0938 | 78.1250 | 41.0156 |  291720 B |
    | CisternLinq |   Reverse |         10000 |   635,570.16 ns |  7,395.8100 ns |  6,918.0453 ns |   639,960.45 ns |  0.40 |    0.02 |  94.7266 | 31.2500 |       - |  341120 B |
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
