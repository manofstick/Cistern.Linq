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
    |      Method | PreSorted | CustomerCount |             Mean |          Error |         StdDev |           Median | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |---------- |-------------- |-----------------:|---------------:|---------------:|-----------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |  SystemLinq |    Random |             0 |         77.07 ns |       1.248 ns |       1.107 ns |         77.34 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |    Random |             0 |        140.56 ns |       1.331 ns |       1.245 ns |        140.66 ns |  1.82 |    0.04 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |    Random |             1 |        174.40 ns |       3.728 ns |       3.487 ns |        174.49 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |    Random |             1 |        396.10 ns |       5.914 ns |       5.532 ns |        395.64 ns |  2.27 |    0.04 |   0.1249 |       - |       - |     392 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |    Random |            10 |      2,309.13 ns |      18.272 ns |      17.092 ns |      2,306.46 ns |  1.00 |    0.00 |   0.2785 |       - |       - |     888 B |
    | CisternLinq |    Random |            10 |      2,017.83 ns |      32.191 ns |      30.111 ns |      2,035.67 ns |  0.87 |    0.02 |   0.2213 |       - |       - |     704 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |    Random |           100 |     56,229.51 ns |     477.194 ns |     446.367 ns |     56,313.10 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |    Random |           100 |     43,032.65 ns |     644.740 ns |     603.090 ns |     43,071.24 ns |  0.77 |    0.01 |   1.2817 |       - |       - |    4080 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |    Random |          1000 |    873,460.43 ns |  11,431.388 ns |  10,692.927 ns |    874,501.56 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |    Random |          1000 |    735,659.31 ns |  13,641.511 ns |  12,092.845 ns |    732,994.87 ns |  0.84 |    0.02 |  11.7188 |       - |       - |   39248 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |    Random |         10000 | 13,070,728.79 ns | 141,088.207 ns | 125,071.020 ns | 13,075,922.66 ns |  1.00 |    0.00 |  93.7500 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |    Random |         10000 | 10,099,582.71 ns | 198,199.022 ns | 185,395.488 ns | 10,100,837.50 ns |  0.77 |    0.02 |  93.7500 | 15.6250 |       - |  381152 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             0 |         78.71 ns |       1.294 ns |       1.147 ns |         78.84 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Forward |             0 |        142.19 ns |       2.218 ns |       2.075 ns |        142.91 ns |  1.81 |    0.04 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |             1 |        182.06 ns |       2.175 ns |       2.034 ns |        182.38 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |   Forward |             1 |        421.90 ns |       5.805 ns |       5.430 ns |        423.07 ns |  2.32 |    0.04 |   0.1249 |       - |       - |     392 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |            10 |      1,168.23 ns |      18.029 ns |      16.864 ns |      1,177.10 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Forward |            10 |      1,284.35 ns |      20.132 ns |      18.831 ns |      1,287.05 ns |  1.10 |    0.02 |   0.2213 |       - |       - |     704 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |           100 |     30,502.31 ns |     406.131 ns |     379.895 ns |     30,631.65 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |   Forward |           100 |     26,044.49 ns |     357.983 ns |     334.857 ns |     26,118.46 ns |  0.85 |    0.02 |   1.2817 |       - |       - |    4080 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |          1000 |    528,261.73 ns |   7,337.001 ns |   6,504.060 ns |    530,941.89 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Forward |          1000 |    472,602.99 ns |   5,966.593 ns |   5,581.155 ns |    476,052.64 ns |  0.89 |    0.01 |  12.2070 |       - |       - |   39248 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Forward |         10000 |  9,119,606.77 ns | 164,802.212 ns | 154,156.091 ns |  9,162,614.06 ns |  1.00 |    0.00 | 109.3750 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |   Forward |         10000 |  7,497,232.81 ns | 101,518.254 ns |  94,960.237 ns |  7,544,258.59 ns |  0.82 |    0.02 |  93.7500 | 31.2500 |       - |  381152 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             0 |         81.46 ns |       3.638 ns |       4.857 ns |         79.25 ns |  1.00 |    0.00 |   0.0356 |       - |       - |     112 B |
    | CisternLinq |   Reverse |             0 |        140.78 ns |       1.571 ns |       1.469 ns |        141.41 ns |  1.71 |    0.11 |   0.0355 |       - |       - |     112 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |             1 |        211.89 ns |       4.780 ns |      10.187 ns |        207.37 ns |  1.00 |    0.00 |   0.1297 |       - |       - |     408 B |
    | CisternLinq |   Reverse |             1 |        410.03 ns |       6.369 ns |       5.958 ns |        408.80 ns |  1.82 |    0.08 |   0.1245 |       - |       - |     392 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |            10 |      3,738.64 ns |      42.794 ns |      35.735 ns |      3,741.61 ns |  1.00 |    0.00 |   0.2823 |       - |       - |     888 B |
    | CisternLinq |   Reverse |            10 |      3,277.14 ns |      29.131 ns |      27.249 ns |      3,278.28 ns |  0.88 |    0.01 |   0.2213 |       - |       - |     704 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |           100 |     44,561.86 ns |     424.052 ns |     396.659 ns |     44,443.44 ns |  1.00 |    0.00 |   1.4038 |       - |       - |    4552 B |
    | CisternLinq |   Reverse |           100 |     36,405.25 ns |     441.588 ns |     391.456 ns |     36,247.69 ns |  0.82 |    0.01 |   1.2817 |       - |       - |    4080 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |          1000 |    840,205.77 ns |   9,182.564 ns |   8,589.376 ns |    839,244.82 ns |  1.00 |    0.00 |  11.7188 |       - |       - |   36960 B |
    | CisternLinq |   Reverse |          1000 |    709,218.88 ns |  11,092.071 ns |  10,375.530 ns |    708,272.56 ns |  0.84 |    0.01 |  11.7188 |       - |       - |   39248 B |
    |             |           |               |                  |                |                |                  |       |         |          |         |         |           |
    |  SystemLinq |   Reverse |         10000 | 13,733,508.09 ns | 313,171.654 ns | 321,604.231 ns | 13,705,104.69 ns |  1.00 |    0.00 | 109.3750 | 31.2500 | 31.2500 |  331720 B |
    | CisternLinq |   Reverse |         10000 | 11,184,868.75 ns | 161,994.924 ns | 151,530.152 ns | 11,178,176.56 ns |  0.81 |    0.02 |  93.7500 | 15.6250 |       - |  381152 B |
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
