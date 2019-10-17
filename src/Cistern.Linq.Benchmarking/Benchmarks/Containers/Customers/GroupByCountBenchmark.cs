using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |         Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |-------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |     107.8 ns |     0.5050 ns |     0.4723 ns |  0.73 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     147.6 ns |     1.2534 ns |     1.1724 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     363.2 ns |     1.5582 ns |     1.4575 ns |  2.46 |    0.02 | 0.1707 |     - |     - |     536 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     206.8 ns |     1.6066 ns |     1.5028 ns |  0.74 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     281.1 ns |     2.1590 ns |     2.0196 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     477.2 ns |     2.7241 ns |     2.5482 ns |  1.70 |    0.01 | 0.2065 |     - |     - |     648 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     835.8 ns |     4.4940 ns |     3.7527 ns |  0.67 |    0.01 | 0.2413 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,252.2 ns |    12.5410 ns |    11.7308 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,250.2 ns |     6.2483 ns |     5.8446 ns |  1.00 |    0.01 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   5,309.9 ns |    46.4845 ns |    43.4816 ns |  0.73 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   7,313.2 ns |    33.9706 ns |    31.7761 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   5,565.3 ns |    39.6139 ns |    37.0548 ns |  0.76 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  46,771.6 ns |   467.0334 ns |   414.0130 ns |  0.89 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  52,594.8 ns |   371.7911 ns |   329.5831 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  26,500.0 ns |   154.0249 ns |   144.0750 ns |  0.50 |    0.00 | 4.6692 |     - |     - |   14800 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     117.4 ns |     0.8507 ns |     0.7957 ns |  0.72 |    0.01 | 0.0455 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     163.1 ns |     1.4226 ns |     1.3307 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     359.5 ns |     2.2585 ns |     2.1126 ns |  2.21 |    0.02 | 0.1702 |     - |     - |     536 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     219.4 ns |     1.2893 ns |     1.2060 ns |  0.74 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     298.2 ns |     2.2697 ns |     2.1231 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     507.1 ns |     5.3899 ns |     5.0417 ns |  1.70 |    0.02 | 0.2050 |     - |     - |     648 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     898.8 ns |     4.8930 ns |     4.5769 ns |  0.67 |    0.00 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,338.0 ns |     8.2608 ns |     7.7271 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,409.6 ns |     8.6617 ns |     8.1022 ns |  1.05 |    0.01 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,822.5 ns |    36.9485 ns |    34.5616 ns |  0.72 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   8,126.3 ns |    30.0069 ns |    28.0685 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   7,202.8 ns |    64.2907 ns |    56.9920 ns |  0.89 |    0.01 | 1.1063 |     - |     - |    3504 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  51,024.4 ns |   309.6014 ns |   289.6013 ns |  0.88 |    0.01 | 0.4272 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  58,185.7 ns |   237.2698 ns |   221.9424 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  40,073.0 ns |   248.2805 ns |   232.2418 ns |  0.69 |    0.00 | 4.6997 |     - |     - |   14800 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     123.4 ns |     1.2935 ns |     1.2100 ns |  0.74 |    0.01 | 0.0532 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     166.0 ns |     0.7965 ns |     0.7451 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     483.7 ns |     6.5270 ns |     6.1053 ns |  2.91 |    0.03 | 0.1898 |     - |     - |     600 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     223.7 ns |     1.8494 ns |     1.7299 ns |  0.74 |    0.01 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     302.4 ns |     2.0434 ns |     1.9114 ns |  1.00 |    0.00 | 0.1760 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     591.8 ns |     4.4796 ns |     4.1903 ns |  1.96 |    0.02 | 0.2270 |     - |     - |     712 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     871.3 ns |     5.7309 ns |     5.3607 ns |  0.70 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,253.1 ns |    10.4967 ns |     9.8186 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,473.8 ns |    10.5735 ns |     9.8905 ns |  1.18 |    0.01 | 0.4768 |     - |     - |    1496 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   5,474.1 ns |    50.2377 ns |    44.5344 ns |  0.71 |    0.01 | 0.4959 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   7,752.3 ns |    47.9752 ns |    44.8760 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   7,129.9 ns |    32.1081 ns |    30.0339 ns |  0.92 |    0.01 | 1.1292 |     - |     - |    3568 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  47,747.1 ns |   310.9438 ns |   290.8570 ns |  0.86 |    0.01 | 0.4272 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  55,422.5 ns |   433.7529 ns |   405.7328 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  37,861.9 ns |   193.8303 ns |   181.3090 ns |  0.68 |    0.01 | 4.6997 |     - |     - |   14864 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     115.7 ns |     0.9068 ns |     0.8482 ns |  0.74 |    0.01 | 0.0329 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     155.8 ns |     1.3587 ns |     1.2709 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     465.4 ns |     4.1084 ns |     3.8430 ns |  2.99 |    0.03 | 0.1707 |     - |     - |     536 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     210.7 ns |     1.3845 ns |     1.2951 ns |  0.74 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     285.5 ns |     1.6471 ns |     1.5407 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     571.8 ns |     5.2719 ns |     4.9313 ns |  2.00 |    0.02 | 0.2060 |     - |     - |     648 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     831.0 ns |     5.0982 ns |     4.7689 ns |  0.65 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,278.9 ns |     6.6856 ns |     6.2538 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,375.8 ns |     4.5570 ns |     4.0397 ns |  1.08 |    0.01 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   5,182.8 ns |    30.3130 ns |    26.8717 ns |  0.70 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   7,381.8 ns |    38.1651 ns |    35.6996 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   5,836.5 ns |    43.1808 ns |    40.3913 ns |  0.79 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  46,140.3 ns |   319.7907 ns |   299.1324 ns |  0.85 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  54,289.1 ns |   329.9683 ns |   308.6525 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  27,724.4 ns |   154.4161 ns |   144.4409 ns |  0.51 |    0.00 | 4.6997 |     - |     - |   14801 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |     111.8 ns |     0.6684 ns |     0.6252 ns |  0.74 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     151.7 ns |     1.8289 ns |     1.7107 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     588.8 ns |     3.9044 ns |     3.6522 ns |  3.88 |    0.06 | 0.1707 |     - |     - |     536 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     555.0 ns |     3.1619 ns |     2.9576 ns |  0.88 |    0.00 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     629.1 ns |     4.3411 ns |     4.0607 ns |  1.00 |    0.00 | 0.1783 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |   1,041.2 ns |     5.1197 ns |     4.7889 ns |  1.66 |    0.01 | 0.2041 |     - |     - |     648 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   2,000.7 ns |    13.9270 ns |    13.0273 ns |  0.80 |    0.01 | 0.2518 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,488.2 ns |    18.7430 ns |    17.5322 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,682.5 ns |    19.4563 ns |    18.1994 ns |  1.08 |    0.01 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  15,098.9 ns |   111.3635 ns |   104.1695 ns |  0.86 |    0.01 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  17,473.6 ns |   134.9803 ns |   126.2607 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  15,782.1 ns |   110.0550 ns |   102.9455 ns |  0.90 |    0.01 | 1.0986 |     - |     - |    3504 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 142,997.0 ns | 1,242.3338 ns | 1,162.0798 ns |  0.97 |    0.01 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 147,230.2 ns | 1,033.2444 ns |   966.4975 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 119,613.2 ns |   821.2051 ns |   768.1558 ns |  0.81 |    0.01 | 4.6387 |     - |     - |   14801 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     117.0 ns |     1.0012 ns |     0.9365 ns |  0.75 |    0.01 | 0.0455 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     156.3 ns |     2.0084 ns |     1.8787 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     504.5 ns |     3.9572 ns |     3.7016 ns |  3.23 |    0.05 | 0.1707 |     - |     - |     536 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     214.3 ns |     1.7119 ns |     1.6013 ns |  0.76 |    0.00 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     283.2 ns |     1.5709 ns |     1.4695 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     643.5 ns |     4.3152 ns |     4.0365 ns |  2.27 |    0.02 | 0.2050 |     - |     - |     648 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     854.9 ns |     6.0414 ns |     5.6511 ns |  0.66 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,298.0 ns |    11.1829 ns |    10.4605 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,549.3 ns |    11.5001 ns |    10.7572 ns |  1.19 |    0.01 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   5,552.2 ns |    53.2381 ns |    49.7989 ns |  0.72 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   7,699.7 ns |    49.7907 ns |    46.5743 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   7,678.9 ns |    35.5951 ns |    33.2957 ns |  1.00 |    0.01 | 1.0986 |     - |     - |    3504 B |
    |             |                |               |              |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  47,825.3 ns |   379.6866 ns |   355.1591 ns |  0.85 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  56,452.7 ns |   302.0446 ns |   282.5327 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  41,870.7 ns |   200.8747 ns |   187.8984 ns |  0.74 |    0.00 | 4.6997 |     - |     - |   14801 B |
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
