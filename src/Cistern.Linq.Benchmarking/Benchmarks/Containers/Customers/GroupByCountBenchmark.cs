using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |       Error |      StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|------------:|------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      98.90 ns |   0.3263 ns |   0.2892 ns |  0.76 |    0.00 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     129.28 ns |   0.9982 ns |   0.9337 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     307.65 ns |   0.5969 ns |   0.5291 ns |  2.38 |    0.01 | 0.1707 |     - |     - |     536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     186.06 ns |   0.4732 ns |   0.4426 ns |  0.75 |    0.00 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     248.76 ns |   1.0263 ns |   0.9600 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     443.54 ns |   1.5291 ns |   1.4303 ns |  1.78 |    0.01 | 0.2494 |     - |     - |     784 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     708.98 ns |   2.7332 ns |   2.5566 ns |  0.66 |    0.00 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,079.50 ns |   3.7533 ns |   3.3272 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,127.74 ns |   4.2474 ns |   3.9730 ns |  1.04 |    0.00 | 0.4864 |     - |     - |    1536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   4,477.47 ns |  18.2427 ns |  17.0643 ns |  0.71 |    0.00 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,286.68 ns |  13.4780 ns |  11.9479 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   4,726.52 ns |  22.1640 ns |  19.6478 ns |  0.75 |    0.00 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  39,571.60 ns | 281.2344 ns | 249.3069 ns |  0.87 |    0.01 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  45,304.38 ns | 134.2650 ns | 119.0225 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  23,081.52 ns | 106.9021 ns |  89.2681 ns |  0.51 |    0.00 | 4.6997 |     - |     - |   14800 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     103.31 ns |   0.3572 ns |   0.3341 ns |  0.75 |    0.00 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     137.35 ns |   0.5087 ns |   0.4509 ns |  1.00 |    0.00 | 0.1121 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     303.14 ns |   0.7818 ns |   0.6931 ns |  2.21 |    0.01 | 0.1707 |     - |     - |     536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     192.13 ns |   0.8238 ns |   0.7303 ns |  0.73 |    0.00 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     264.99 ns |   0.6606 ns |   0.6179 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     475.36 ns |   2.0212 ns |   1.8906 ns |  1.79 |    0.01 | 0.2489 |     - |     - |     784 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     769.57 ns |   2.6508 ns |   2.2136 ns |  0.68 |    0.00 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,128.17 ns |   4.4721 ns |   4.1832 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,279.17 ns |   3.9890 ns |   3.5361 ns |  1.13 |    0.01 | 0.4864 |     - |     - |    1536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   4,937.63 ns |  13.0594 ns |  12.2157 ns |  0.73 |    0.00 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   6,736.36 ns |  24.3120 ns |  21.5519 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   6,229.56 ns |  15.3902 ns |  13.6430 ns |  0.92 |    0.00 | 1.1063 |     - |     - |    3504 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  43,182.87 ns | 109.8773 ns |  97.4034 ns |  0.89 |    0.00 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  48,597.92 ns | 133.3895 ns | 118.2463 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  34,900.28 ns | 116.4170 ns | 103.2006 ns |  0.72 |    0.00 | 4.6997 |     - |     - |   14800 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     109.24 ns |   1.0332 ns |   0.9665 ns |  0.78 |    0.01 | 0.0535 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     139.86 ns |   0.7464 ns |   0.6617 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     407.02 ns |   1.5084 ns |   1.4110 ns |  2.91 |    0.02 | 0.1912 |     - |     - |     600 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     190.13 ns |   0.5709 ns |   0.5340 ns |  0.73 |    0.00 | 0.1249 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     261.18 ns |   0.7679 ns |   0.7183 ns |  1.00 |    0.00 | 0.1760 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     525.84 ns |   1.7625 ns |   1.6486 ns |  2.01 |    0.00 | 0.2699 |     - |     - |     848 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     754.09 ns |   1.2876 ns |   1.2044 ns |  0.67 |    0.00 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,124.45 ns |   6.9210 ns |   6.4739 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,333.08 ns |   3.3542 ns |   2.9734 ns |  1.19 |    0.01 | 0.5093 |     - |     - |    1600 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   4,752.41 ns |   6.9713 ns |   6.5210 ns |  0.74 |    0.00 | 0.5035 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   6,456.33 ns |  16.6274 ns |  14.7398 ns |  1.00 |    0.00 | 1.6632 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   6,059.10 ns |  14.0190 ns |  12.4275 ns |  0.94 |    0.00 | 1.1368 |     - |     - |    3568 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  41,326.35 ns | 152.8657 ns | 119.3475 ns |  0.87 |    0.00 | 0.4883 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  47,468.52 ns | 149.4639 ns | 132.4958 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  32,447.59 ns | 106.8482 ns |  99.9459 ns |  0.68 |    0.00 | 4.6997 |     - |     - |   14864 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |      97.63 ns |   0.8269 ns |   0.7734 ns |  0.76 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     129.21 ns |   0.2707 ns |   0.2400 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     396.87 ns |   2.0753 ns |   1.9413 ns |  3.07 |    0.01 | 0.1707 |     - |     - |     536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     180.21 ns |   0.5085 ns |   0.4756 ns |  0.73 |    0.00 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     245.83 ns |   0.8381 ns |   0.7839 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     524.22 ns |   3.5654 ns |   3.1607 ns |  2.13 |    0.01 | 0.2499 |     - |     - |     784 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     701.48 ns |   2.3919 ns |   2.1204 ns |  0.65 |    0.00 | 0.2413 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,078.48 ns |   5.1661 ns |   4.3139 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,227.62 ns |   3.5173 ns |   3.2900 ns |  1.14 |    0.00 | 0.4864 |     - |     - |    1536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   4,469.88 ns |  12.1423 ns |  11.3579 ns |  0.71 |    0.00 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,273.91 ns |  17.3714 ns |  15.3993 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   4,922.79 ns |  13.9047 ns |  13.0065 ns |  0.78 |    0.00 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  40,056.87 ns | 147.4903 ns | 137.9625 ns |  0.87 |    0.00 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  45,898.88 ns | 192.1441 ns | 179.7317 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  23,515.66 ns |  91.3189 ns |  85.4198 ns |  0.51 |    0.00 | 4.6997 |     - |     - |   14801 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      96.47 ns |   1.0360 ns |   0.9690 ns |  0.74 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     130.56 ns |   0.6130 ns |   0.5734 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     493.02 ns |   1.3220 ns |   1.1720 ns |  3.78 |    0.02 | 0.1698 |     - |     - |     536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     490.98 ns |   1.2220 ns |   1.0833 ns |  0.91 |    0.00 | 0.1259 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     537.39 ns |   1.4120 ns |   1.3207 ns |  1.00 |    0.00 | 0.1783 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |     897.41 ns |   2.5300 ns |   2.3666 ns |  1.67 |    0.00 | 0.2489 |     - |     - |     784 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,744.51 ns |   7.0064 ns |   6.5538 ns |  0.83 |    0.00 | 0.2518 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,108.34 ns |   4.9934 ns |   4.6708 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,377.09 ns |   7.0877 ns |   6.6298 ns |  1.13 |    0.00 | 0.4845 |     - |     - |    1536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  12,793.38 ns |  29.6973 ns |  26.3259 ns |  0.85 |    0.00 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  14,971.03 ns |  38.5896 ns |  36.0967 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  13,652.49 ns |  62.2535 ns |  58.2320 ns |  0.91 |    0.00 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 123,783.34 ns | 292.5911 ns | 273.6899 ns |  0.96 |    0.00 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 128,297.74 ns | 391.3245 ns | 326.7736 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 107,440.06 ns | 236.0425 ns | 220.7943 ns |  0.84 |    0.00 | 4.6387 |     - |     - |   14801 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |      95.60 ns |   0.2466 ns |   0.2306 ns |  0.70 |    0.00 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     135.69 ns |   0.4203 ns |   0.3726 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     421.76 ns |   1.1749 ns |   1.0990 ns |  3.11 |    0.02 | 0.1702 |     - |     - |     536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     187.76 ns |   0.5187 ns |   0.4598 ns |  0.76 |    0.00 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     245.76 ns |   1.6777 ns |   1.5693 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     578.86 ns |   1.5010 ns |   1.2534 ns |  2.36 |    0.01 | 0.2489 |     - |     - |     784 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     728.21 ns |   2.5835 ns |   2.2902 ns |  0.67 |    0.00 | 0.2432 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,089.98 ns |   2.7672 ns |   2.4531 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,384.15 ns |   3.9698 ns |   3.7134 ns |  1.27 |    0.00 | 0.4883 |     - |     - |    1536 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   4,805.54 ns |  29.5731 ns |  27.6627 ns |  0.73 |    0.00 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   6,578.59 ns |  15.2240 ns |  14.2405 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   6,459.75 ns |  15.1196 ns |  14.1428 ns |  0.98 |    0.00 | 1.1063 |     - |     - |    3504 B |
    |             |                |               |               |             |             |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  41,566.17 ns |  89.3226 ns |  83.5524 ns |  0.87 |    0.00 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  47,898.39 ns | 118.7842 ns | 105.2991 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  35,086.23 ns |  96.9923 ns |  90.7267 ns |  0.73 |    0.00 | 4.6387 |     - |     - |   14801 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_GroupByCountBenchmark : CustomersBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var counts = new Dictionary<string, int>();
            foreach (var c in Customers)
            {
                counts.TryGetValue(c.State, out var count);
                counts[c.State] = count + 1;
            }

            var results = new DesiredShape();
            foreach (var c in counts)
                results.Add((c.Key, c.Value));
            return results;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.Select(
                        System.Linq.Enumerable.GroupBy(
                            Customers,
                            c => c.State),
                        c => (c.Key, c.Count()))
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Customers
                .GroupBy(c => c.State)
                .Select(c => (c.Key, c.Count()))
                .ToList();
        }

    }
}
