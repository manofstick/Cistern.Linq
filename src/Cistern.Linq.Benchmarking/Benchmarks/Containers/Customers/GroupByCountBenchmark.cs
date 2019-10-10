using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      99.93 ns |     2.0279 ns |     2.4905 ns |  0.73 |    0.02 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     135.15 ns |     1.5330 ns |     1.4340 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     333.77 ns |     3.0508 ns |     2.8537 ns |  2.47 |    0.03 | 0.1984 |     - |     - |     624 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     190.20 ns |     1.6900 ns |     1.5808 ns |  0.74 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     257.97 ns |     3.2923 ns |     3.0796 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     571.63 ns |     7.4928 ns |     7.0088 ns |  2.22 |    0.04 | 0.2337 |     - |     - |     736 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     763.58 ns |     2.6673 ns |     2.3645 ns |  0.67 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,147.18 ns |    16.9409 ns |    15.8466 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,423.63 ns |    17.7162 ns |    15.7050 ns |  1.24 |    0.03 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   4,721.72 ns |    13.8565 ns |    12.2834 ns |  0.73 |    0.01 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,468.67 ns |    66.6817 ns |    62.3741 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   6,332.67 ns |    66.1457 ns |    61.8727 ns |  0.98 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  41,410.55 ns |   561.6745 ns |   525.3906 ns |  0.88 |    0.01 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  46,902.75 ns |   635.0011 ns |   530.2547 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  32,950.74 ns |   295.0260 ns |   275.9675 ns |  0.70 |    0.01 | 4.6387 |     - |     - |   14800 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     107.54 ns |     1.9592 ns |     1.6360 ns |  0.73 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     147.83 ns |     1.5691 ns |     1.4677 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     332.05 ns |     3.1025 ns |     2.9021 ns |  2.25 |    0.03 | 0.1988 |     - |     - |     624 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     196.77 ns |     2.0429 ns |     1.9110 ns |  0.73 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     269.67 ns |     4.0200 ns |     3.7603 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     590.31 ns |     8.7608 ns |     7.7662 ns |  2.19 |    0.05 | 0.2346 |     - |     - |     736 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     803.83 ns |     7.8726 ns |     7.3640 ns |  0.67 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,191.75 ns |    13.7841 ns |    12.8937 ns |  1.00 |    0.00 | 0.4234 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,452.34 ns |    17.0419 ns |    15.9410 ns |  1.22 |    0.02 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,081.95 ns |    60.9502 ns |    57.0128 ns |  0.73 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   6,997.82 ns |    70.1851 ns |    65.6512 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   6,909.77 ns |    83.6715 ns |    78.2664 ns |  0.99 |    0.02 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  44,268.46 ns |   251.3116 ns |   235.0771 ns |  0.86 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  51,585.25 ns |   466.3830 ns |   436.2550 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  38,305.60 ns |   505.6319 ns |   472.9684 ns |  0.74 |    0.01 | 4.6997 |     - |     - |   14800 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     115.62 ns |     1.3260 ns |     1.2403 ns |  0.78 |    0.01 | 0.0532 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     147.51 ns |     1.6063 ns |     1.5025 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     417.42 ns |     5.2572 ns |     4.9176 ns |  2.83 |    0.05 | 0.2193 |     - |     - |     688 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     206.53 ns |     2.2559 ns |     2.1101 ns |  0.75 |    0.01 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     274.33 ns |     3.2183 ns |     3.0104 ns |  1.00 |    0.00 | 0.1755 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     671.15 ns |     6.0608 ns |     5.6692 ns |  2.45 |    0.04 | 0.2546 |     - |     - |     800 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     778.67 ns |    11.5608 ns |    10.8140 ns |  0.67 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,157.32 ns |    15.9374 ns |    14.9079 ns |  1.00 |    0.00 | 0.4311 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,577.17 ns |    19.4904 ns |    18.2314 ns |  1.36 |    0.02 | 0.4749 |     - |     - |    1496 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   4,968.66 ns |    89.9545 ns |    75.1161 ns |  0.72 |    0.02 | 0.5035 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   6,914.51 ns |    90.3561 ns |    84.5192 ns |  1.00 |    0.00 | 1.6632 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   7,366.44 ns |   110.7355 ns |   103.5821 ns |  1.07 |    0.02 | 1.1292 |     - |     - |    3568 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  43,060.65 ns |   416.1405 ns |   389.2581 ns |  0.87 |    0.01 | 0.4883 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  49,725.34 ns |   704.2227 ns |   658.7304 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  40,984.79 ns |   593.1862 ns |   554.8667 ns |  0.82 |    0.01 | 4.6387 |     - |     - |   14864 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     101.53 ns |     1.4315 ns |     1.3391 ns |  0.74 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     136.34 ns |     1.3859 ns |     1.2964 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     443.63 ns |     6.7590 ns |     6.3224 ns |  3.25 |    0.06 | 0.1988 |     - |     - |     624 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     191.24 ns |     1.9013 ns |     1.7785 ns |  0.74 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     260.17 ns |     3.3685 ns |     3.1509 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     668.93 ns |     9.0425 ns |     8.4584 ns |  2.57 |    0.05 | 0.2337 |     - |     - |     736 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     750.73 ns |     8.0568 ns |     7.5363 ns |  0.66 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,138.94 ns |    13.7241 ns |    12.8376 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,518.95 ns |    13.7103 ns |    12.8247 ns |  1.33 |    0.02 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   4,682.78 ns |    75.7387 ns |    70.8460 ns |  0.71 |    0.02 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,569.00 ns |    99.6680 ns |    93.2295 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   6,615.44 ns |    81.2084 ns |    75.9624 ns |  1.01 |    0.02 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  41,153.99 ns |   410.8211 ns |   384.2823 ns |  0.86 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  48,075.80 ns |   582.6916 ns |   545.0501 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  33,936.74 ns |   443.0007 ns |   414.3831 ns |  0.71 |    0.01 | 4.6387 |     - |     - |   14801 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |     102.21 ns |     1.8821 ns |     1.7605 ns |  0.75 |    0.02 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     136.39 ns |     1.6188 ns |     1.5142 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     542.28 ns |     8.0373 ns |     7.5181 ns |  3.98 |    0.06 | 0.1984 |     - |     - |     624 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     505.79 ns |     6.5630 ns |     6.1390 ns |  0.91 |    0.01 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     556.43 ns |     8.5441 ns |     7.9921 ns |  1.00 |    0.00 | 0.1774 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |   1,046.70 ns |    19.5418 ns |    18.2794 ns |  1.88 |    0.04 | 0.2346 |     - |     - |     736 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,837.32 ns |    17.9770 ns |    16.8157 ns |  0.85 |    0.01 | 0.2537 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,145.42 ns |    23.9533 ns |    20.0021 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,761.99 ns |    26.7073 ns |    24.9820 ns |  1.29 |    0.02 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  13,847.00 ns |   141.0108 ns |   131.9016 ns |  0.88 |    0.02 | 0.5035 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  15,693.35 ns |   293.9526 ns |   314.5260 ns |  1.00 |    0.00 | 1.6174 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  15,722.48 ns |   251.5502 ns |   235.3002 ns |  1.00 |    0.03 | 1.0986 |     - |     - |    3504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 128,312.21 ns | 1,839.6298 ns | 1,720.7908 ns |  0.95 |    0.01 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 135,759.32 ns | 1,283.0946 ns | 1,200.2074 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 120,887.66 ns | 1,902.0149 ns | 1,779.1460 ns |  0.89 |    0.01 | 4.3945 |     - |     - |   14801 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     101.47 ns |     0.9432 ns |     0.8823 ns |  0.71 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     142.54 ns |     1.6984 ns |     1.5887 ns |  1.00 |    0.00 | 0.1121 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     433.29 ns |     3.9761 ns |     3.5247 ns |  3.04 |    0.03 | 0.1984 |     - |     - |     624 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     191.92 ns |     2.3455 ns |     2.1940 ns |  0.74 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     258.48 ns |     2.6603 ns |     2.4884 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     667.62 ns |     9.1341 ns |     8.5441 ns |  2.58 |    0.04 | 0.2346 |     - |     - |     736 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     784.22 ns |    12.9438 ns |    12.1077 ns |  0.69 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,136.01 ns |    14.3609 ns |    13.4332 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,714.29 ns |    18.9835 ns |    17.7572 ns |  1.51 |    0.02 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   4,914.37 ns |    74.0536 ns |    69.2698 ns |  0.73 |    0.02 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   6,713.86 ns |    81.8878 ns |    76.5979 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   7,750.40 ns |    67.1677 ns |    59.5424 ns |  1.15 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  42,738.72 ns |   571.6440 ns |   534.7162 ns |  0.86 |    0.02 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  49,772.36 ns |   575.9882 ns |   538.7797 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  43,996.86 ns |   845.0589 ns |   904.2036 ns |  0.88 |    0.02 | 4.6997 |     - |     - |   14801 B |
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
