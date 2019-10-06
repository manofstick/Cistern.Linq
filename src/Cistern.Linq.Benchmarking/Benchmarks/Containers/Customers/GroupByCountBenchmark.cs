using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      98.32 ns |     0.4539 ns |     0.4246 ns |  0.75 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     130.64 ns |     2.0743 ns |     1.9403 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     232.48 ns |     2.5931 ns |     2.2987 ns |  1.78 |    0.03 | 0.1605 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     189.39 ns |     1.8891 ns |     1.7671 ns |  0.74 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     255.35 ns |     3.4451 ns |     3.2226 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     432.26 ns |     4.9886 ns |     4.6664 ns |  1.69 |    0.03 | 0.1960 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     763.20 ns |     7.8067 ns |     6.9204 ns |  0.68 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,124.51 ns |    19.2396 ns |    17.9967 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,202.96 ns |    19.1958 ns |    17.9558 ns |  1.07 |    0.03 | 0.4177 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   5,012.66 ns |    46.7203 ns |    43.7022 ns |  0.76 |    0.01 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,576.25 ns |    79.2624 ns |    74.1421 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   5,796.94 ns |    81.5576 ns |    76.2890 ns |  0.88 |    0.02 | 1.2360 |     - |     - |    3896 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  51,723.36 ns |   535.9083 ns |   475.0687 ns |  0.97 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  53,270.67 ns |   376.6343 ns |   333.8765 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  46,321.11 ns |   549.3732 ns |   487.0051 ns |  0.87 |    0.01 | 8.1177 |     - |     - |   25472 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     106.77 ns |     1.4015 ns |     1.3110 ns |  0.77 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     139.49 ns |     1.5867 ns |     1.4842 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     233.26 ns |     2.6602 ns |     2.4883 ns |  1.67 |    0.03 | 0.1605 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     203.91 ns |     1.3655 ns |     1.2105 ns |  0.75 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     272.99 ns |     2.7439 ns |     2.5667 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     464.59 ns |     5.3378 ns |     4.9930 ns |  1.70 |    0.02 | 0.1960 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     811.21 ns |    11.3727 ns |    10.6381 ns |  0.69 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,177.50 ns |    13.2530 ns |    11.7485 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,276.87 ns |    14.6228 ns |    13.6782 ns |  1.08 |    0.01 | 0.4158 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,930.23 ns |    48.5642 ns |    45.4269 ns |  0.80 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   7,415.87 ns |   106.5364 ns |    99.6543 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   7,073.87 ns |   122.3104 ns |   108.4249 ns |  0.95 |    0.02 | 1.2360 |     - |     - |    3896 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  55,464.58 ns |   894.5947 ns |   836.8044 ns |  0.90 |    0.02 | 0.4272 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  61,711.96 ns |   977.9936 ns |   914.8158 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  60,847.64 ns |   768.9199 ns |   719.2481 ns |  0.99 |    0.02 | 8.1177 |     - |     - |   25472 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     110.30 ns |     1.7657 ns |     1.6516 ns |  0.75 |    0.01 | 0.0534 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     147.44 ns |     2.0110 ns |     1.8810 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     320.92 ns |     4.4207 ns |     4.1351 ns |  2.18 |    0.04 | 0.1807 |     - |     - |     568 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     200.91 ns |     3.0832 ns |     2.8840 ns |  0.74 |    0.01 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     270.87 ns |     3.3298 ns |     3.1147 ns |  1.00 |    0.00 | 0.1755 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     562.15 ns |     6.4200 ns |     6.0053 ns |  2.08 |    0.04 | 0.2165 |     - |     - |     680 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     812.22 ns |     9.9179 ns |     9.2772 ns |  0.70 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,156.88 ns |    15.6237 ns |    14.6144 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,399.23 ns |    16.8908 ns |    15.7997 ns |  1.21 |    0.02 | 0.4387 |     - |     - |    1376 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   5,536.99 ns |    68.6825 ns |    64.2457 ns |  0.78 |    0.01 | 0.4959 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   7,110.03 ns |    85.8411 ns |    80.2958 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   7,168.28 ns |    88.3343 ns |    82.6280 ns |  1.01 |    0.02 | 1.2589 |     - |     - |    3960 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  51,983.35 ns |   699.7550 ns |   654.5513 ns |  0.88 |    0.01 | 0.4272 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  58,912.16 ns |   848.5365 ns |   793.7216 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  61,975.17 ns |   930.6421 ns |   870.5232 ns |  1.05 |    0.02 | 7.9346 |     - |     - |   25536 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     102.21 ns |     1.1033 ns |     1.0320 ns |  0.72 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     142.19 ns |     2.2312 ns |     2.0870 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     349.42 ns |     3.4539 ns |     3.2308 ns |  2.46 |    0.04 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     191.18 ns |     2.7427 ns |     2.5656 ns |  0.74 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     257.31 ns |     3.7074 ns |     3.4679 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     549.25 ns |     8.6810 ns |     8.1202 ns |  2.13 |    0.03 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     777.58 ns |     8.9939 ns |     8.4129 ns |  0.69 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,131.86 ns |    13.0491 ns |    12.2062 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,286.52 ns |    15.5785 ns |    14.5722 ns |  1.14 |    0.02 | 0.4158 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   5,039.48 ns |    53.8070 ns |    50.3311 ns |  0.76 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,623.63 ns |    57.7185 ns |    53.9899 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   5,905.37 ns |    65.3999 ns |    61.1751 ns |  0.89 |    0.01 | 1.2360 |     - |     - |    3896 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  51,159.14 ns |   522.0448 ns |   488.3210 ns |  0.89 |    0.01 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  57,591.14 ns |   703.0558 ns |   657.6388 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  48,406.94 ns |   458.8228 ns |   429.1831 ns |  0.84 |    0.01 | 8.0566 |     - |     - |   25474 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      99.93 ns |     2.0245 ns |     1.8937 ns |  0.72 |    0.02 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     138.35 ns |     1.5115 ns |     1.4139 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     428.87 ns |     4.5863 ns |     4.2901 ns |  3.10 |    0.03 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     496.18 ns |     4.3639 ns |     4.0820 ns |  0.88 |    0.01 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     566.08 ns |     5.4537 ns |     5.1014 ns |  1.00 |    0.00 | 0.1774 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |     915.17 ns |     5.5434 ns |     5.1853 ns |  1.62 |    0.02 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,824.41 ns |    11.4912 ns |    10.1866 ns |  0.84 |    0.01 | 0.2537 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,169.19 ns |    30.1012 ns |    28.1567 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,505.45 ns |    37.7108 ns |    35.2747 ns |  1.16 |    0.02 | 0.4158 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  13,712.66 ns |   187.7719 ns |   175.6420 ns |  0.85 |    0.02 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  16,194.82 ns |   235.0942 ns |   219.9073 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  15,759.93 ns |   226.7258 ns |   212.0795 ns |  0.97 |    0.02 | 1.1902 |     - |     - |    3896 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 134,730.87 ns | 1,568.6107 ns | 1,467.2794 ns |  0.93 |    0.02 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 144,457.43 ns | 1,988.0881 ns | 1,859.6588 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 146,672.50 ns | 1,518.1331 ns | 1,420.0626 ns |  1.02 |    0.02 | 7.8125 |     - |     - |   25474 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     101.85 ns |     1.3584 ns |     1.2042 ns |  0.74 |    0.02 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     137.45 ns |     1.5256 ns |     1.4270 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     355.12 ns |     2.9821 ns |     2.7895 ns |  2.58 |    0.03 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     193.87 ns |     2.3806 ns |     2.2269 ns |  0.74 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     261.81 ns |     3.5094 ns |     3.2827 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     556.17 ns |     5.6869 ns |     5.3195 ns |  2.12 |    0.03 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     788.34 ns |     8.8150 ns |     8.2456 ns |  0.68 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,153.48 ns |    14.2967 ns |    13.3732 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,391.15 ns |    12.3439 ns |    11.5465 ns |  1.21 |    0.02 | 0.4177 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   5,493.79 ns |    76.6174 ns |    71.6680 ns |  0.77 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   7,164.43 ns |    75.7839 ns |    70.8883 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   7,381.28 ns |    93.7776 ns |    87.7196 ns |  1.03 |    0.02 | 1.2360 |     - |     - |    3896 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  54,635.83 ns |   629.7522 ns |   589.0706 ns |  0.87 |    0.02 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  63,159.35 ns | 1,132.5180 ns | 1,059.3580 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  60,132.37 ns |   904.9884 ns |   846.5268 ns |  0.95 |    0.02 | 8.0566 |     - |     - |   25474 B |
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
