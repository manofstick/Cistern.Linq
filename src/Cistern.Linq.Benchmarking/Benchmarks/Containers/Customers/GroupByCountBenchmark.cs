using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |        Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|-------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      97.52 ns |     2.005 ns |     3.1215 ns |  0.76 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     131.52 ns |     1.775 ns |     1.6605 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     223.35 ns |     2.696 ns |     2.5214 ns |  1.70 |    0.03 | 0.1581 |     - |     - |     496 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     186.60 ns |     1.743 ns |     1.5448 ns |  0.73 |    0.01 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     255.53 ns |     3.062 ns |     2.8645 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     433.10 ns |     5.627 ns |     5.2635 ns |  1.70 |    0.03 | 0.1931 |     - |     - |     608 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     763.10 ns |     6.704 ns |     6.2713 ns |  0.69 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,110.26 ns |    13.587 ns |    12.7097 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,163.53 ns |    13.419 ns |    12.5523 ns |  1.05 |    0.02 | 0.4139 |     - |     - |    1304 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   5,046.96 ns |    34.214 ns |    32.0035 ns |  0.76 |    0.01 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,605.00 ns |    86.047 ns |    80.4888 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   5,602.01 ns |    56.782 ns |    53.1141 ns |  0.85 |    0.01 | 1.2283 |     - |     - |    3888 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  50,854.97 ns |   779.130 ns |   728.7990 ns |  0.94 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  54,062.14 ns |   262.656 ns |   232.8373 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  47,626.78 ns |   589.847 ns |   551.7436 ns |  0.88 |    0.01 | 8.1177 |     - |     - |   25464 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     108.51 ns |     1.872 ns |     1.6592 ns |  0.78 |    0.02 | 0.0459 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     139.56 ns |     1.522 ns |     1.4241 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     223.54 ns |     2.141 ns |     1.8981 ns |  1.60 |    0.02 | 0.1578 |     - |     - |     496 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     198.70 ns |     2.395 ns |     2.2399 ns |  0.75 |    0.01 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     264.95 ns |     2.294 ns |     2.1457 ns |  1.00 |    0.00 | 0.1678 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     448.45 ns |     6.181 ns |     5.4789 ns |  1.69 |    0.03 | 0.1936 |     - |     - |     608 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     826.18 ns |     6.083 ns |     5.6900 ns |  0.71 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,155.64 ns |    13.865 ns |    12.9691 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,334.47 ns |     6.003 ns |     5.6155 ns |  1.15 |    0.01 | 0.4139 |     - |     - |    1304 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,604.89 ns |    45.968 ns |    42.9980 ns |  0.76 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   7,349.66 ns |    67.156 ns |    62.8174 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   6,911.06 ns |    80.542 ns |    71.3983 ns |  0.94 |    0.01 | 1.2283 |     - |     - |    3888 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  56,015.64 ns |   808.446 ns |   756.2206 ns |  0.93 |    0.02 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  60,418.42 ns |   816.057 ns |   763.3403 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  58,446.83 ns |   836.850 ns |   782.7903 ns |  0.97 |    0.02 | 8.1177 |     - |     - |   25464 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     111.58 ns |     2.047 ns |     1.9148 ns |  0.77 |    0.02 | 0.0534 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     144.61 ns |     2.683 ns |     2.5098 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     322.63 ns |     3.895 ns |     3.6431 ns |  2.23 |    0.04 | 0.1783 |     - |     - |     560 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     201.76 ns |     1.113 ns |     0.9862 ns |  0.76 |    0.01 | 0.1249 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     266.20 ns |     3.168 ns |     2.9630 ns |  1.00 |    0.00 | 0.1760 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     516.13 ns |     6.335 ns |     5.9257 ns |  1.94 |    0.03 | 0.2127 |     - |     - |     672 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     799.78 ns |     9.809 ns |     9.1751 ns |  0.70 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,136.67 ns |    11.343 ns |    10.6104 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,346.44 ns |    12.687 ns |    11.8678 ns |  1.18 |    0.01 | 0.4349 |     - |     - |    1368 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   5,498.81 ns |    72.914 ns |    68.2036 ns |  0.79 |    0.01 | 0.5035 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   6,983.74 ns |   117.139 ns |    91.4541 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   6,895.76 ns |   108.160 ns |   101.1728 ns |  0.99 |    0.02 | 1.2589 |     - |     - |    3952 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  51,931.93 ns |   889.746 ns |   832.2692 ns |  0.88 |    0.01 | 0.4883 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  58,785.06 ns |   793.258 ns |   742.0144 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  56,884.26 ns |   697.810 ns |   652.7319 ns |  0.97 |    0.01 | 8.0566 |     - |     - |   25528 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     100.20 ns |     1.235 ns |     1.1548 ns |  0.70 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     142.16 ns |     1.474 ns |     1.3786 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     338.59 ns |     3.400 ns |     3.1807 ns |  2.38 |    0.03 | 0.1578 |     - |     - |     496 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     186.81 ns |     1.539 ns |     1.4397 ns |  0.72 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     259.45 ns |     4.300 ns |     4.0221 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     537.50 ns |     6.427 ns |     6.0114 ns |  2.07 |    0.04 | 0.1936 |     - |     - |     608 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     784.26 ns |    10.114 ns |     9.4611 ns |  0.69 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,128.87 ns |    13.039 ns |    12.1967 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,276.96 ns |    15.117 ns |    14.1401 ns |  1.13 |    0.02 | 0.4139 |     - |     - |    1304 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   4,947.07 ns |    67.580 ns |    63.2140 ns |  0.74 |    0.02 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,685.19 ns |   107.703 ns |   100.7455 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   5,596.14 ns |    42.719 ns |    39.9597 ns |  0.84 |    0.01 | 1.2360 |     - |     - |    3888 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  51,324.88 ns |   601.044 ns |   562.2170 ns |  0.85 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  60,272.92 ns |   667.692 ns |   624.5599 ns |  1.00 |    0.00 | 8.6670 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  48,206.61 ns |   362.823 ns |   339.3852 ns |  0.80 |    0.01 | 8.1177 |     - |     - |   25466 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      98.31 ns |     1.470 ns |     1.3754 ns |  0.71 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     139.30 ns |     1.765 ns |     1.6513 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     419.66 ns |     5.721 ns |     5.3519 ns |  3.01 |    0.04 | 0.1578 |     - |     - |     496 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     495.31 ns |     7.037 ns |     6.5822 ns |  0.88 |    0.01 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     565.59 ns |     6.229 ns |     5.8264 ns |  1.00 |    0.00 | 0.1783 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |     869.07 ns |     8.426 ns |     7.8816 ns |  1.54 |    0.02 | 0.1936 |     - |     - |     608 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,715.94 ns |    10.886 ns |     9.0903 ns |  0.79 |    0.01 | 0.2537 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,167.44 ns |    26.353 ns |    24.6510 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,545.79 ns |    27.859 ns |    26.0596 ns |  1.17 |    0.02 | 0.4120 |     - |     - |    1304 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  13,714.64 ns |   199.513 ns |   186.6248 ns |  0.84 |    0.02 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  16,287.43 ns |   140.881 ns |   131.7804 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  15,453.83 ns |   228.390 ns |   213.6361 ns |  0.95 |    0.02 | 1.2360 |     - |     - |    3888 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 134,696.59 ns | 1,917.620 ns | 1,793.7425 ns |  0.95 |    0.02 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 141,730.73 ns | 2,166.497 ns | 2,026.5429 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 138,562.56 ns | 1,783.307 ns | 1,668.1064 ns |  0.98 |    0.02 | 8.0566 |     - |     - |   25466 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     100.18 ns |     1.447 ns |     1.3535 ns |  0.73 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     138.10 ns |     1.923 ns |     1.7989 ns |  1.00 |    0.00 | 0.1121 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     340.93 ns |     6.167 ns |     5.7687 ns |  2.47 |    0.06 | 0.1574 |     - |     - |     496 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     188.39 ns |     1.748 ns |     1.6352 ns |  0.72 |    0.01 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     260.25 ns |     3.496 ns |     3.2699 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     537.30 ns |     6.372 ns |     5.9606 ns |  2.06 |    0.04 | 0.1936 |     - |     - |     608 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     788.98 ns |    12.713 ns |    11.8919 ns |  0.70 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,126.89 ns |    14.421 ns |    13.4894 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,373.63 ns |    12.455 ns |    11.6506 ns |  1.22 |    0.02 | 0.4139 |     - |     - |    1304 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   5,570.74 ns |    71.473 ns |    66.8563 ns |  0.79 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   7,076.68 ns |    72.132 ns |    67.4726 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   7,280.42 ns |    86.869 ns |    81.2571 ns |  1.03 |    0.01 | 1.2283 |     - |     - |    3888 B |
    |             |                |               |               |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  54,981.43 ns |   957.598 ns |   895.7380 ns |  0.91 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  60,671.53 ns |   862.476 ns |   806.7609 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  57,804.00 ns |   879.289 ns |   822.4873 ns |  0.95 |    0.02 | 8.1177 |     - |     - |   25466 B |
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
