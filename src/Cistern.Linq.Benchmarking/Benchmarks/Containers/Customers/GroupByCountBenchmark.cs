using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |        Error |       StdDev |        Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|-------------:|-------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      98.04 ns |     1.338 ns |     1.251 ns |      98.15 ns |  0.74 |    0.02 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     132.70 ns |     2.032 ns |     1.900 ns |     133.22 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     322.66 ns |     2.881 ns |     2.694 ns |     322.50 ns |  2.43 |    0.03 | 0.1755 |     - |     - |     552 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     186.56 ns |     3.280 ns |     3.068 ns |     186.80 ns |  0.71 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     261.05 ns |     3.181 ns |     2.975 ns |     262.15 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     553.00 ns |     7.707 ns |     7.209 ns |     554.62 ns |  2.12 |    0.04 | 0.2108 |     - |     - |     664 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     737.66 ns |     9.427 ns |     8.818 ns |     738.86 ns |  0.66 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,119.28 ns |    14.753 ns |    13.800 ns |   1,117.78 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,322.55 ns |    12.156 ns |    10.776 ns |   1,324.30 ns |  1.18 |    0.01 | 0.4311 |     - |     - |    1360 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   4,677.07 ns |    65.533 ns |    61.299 ns |   4,710.63 ns |  0.71 |    0.01 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,547.52 ns |    65.486 ns |    61.255 ns |   6,555.41 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   5,510.24 ns |    34.922 ns |    30.957 ns |   5,520.79 ns |  0.84 |    0.01 | 1.0910 |     - |     - |    3432 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  43,175.38 ns |   856.426 ns | 1,454.276 ns |  43,191.05 ns |  0.85 |    0.05 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  50,230.94 ns |   424.243 ns |   396.837 ns |  50,219.03 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  28,554.81 ns |   288.690 ns |   270.041 ns |  28,600.74 ns |  0.57 |    0.01 | 4.6387 |     - |     - |   14728 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     112.02 ns |     1.458 ns |     1.138 ns |     111.76 ns |  0.72 |    0.01 | 0.0459 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     154.82 ns |     2.966 ns |     2.913 ns |     155.44 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     336.67 ns |     3.356 ns |     3.139 ns |     335.83 ns |  2.18 |    0.05 | 0.1755 |     - |     - |     552 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     206.74 ns |     4.197 ns |     4.122 ns |     206.49 ns |  0.71 |    0.02 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     290.10 ns |     3.341 ns |     2.962 ns |     290.52 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     580.90 ns |    10.253 ns |     9.591 ns |     578.33 ns |  2.00 |    0.04 | 0.2108 |     - |     - |     664 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     828.78 ns |     8.994 ns |     7.973 ns |     826.17 ns |  0.66 |    0.03 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,241.11 ns |    24.886 ns |    51.946 ns |   1,218.46 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,409.15 ns |    20.223 ns |    18.917 ns |   1,404.52 ns |  1.12 |    0.04 | 0.4330 |     - |     - |    1360 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,075.27 ns |    59.379 ns |    52.638 ns |   5,074.20 ns |  0.71 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   7,138.93 ns |    39.630 ns |    37.070 ns |   7,137.31 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   6,509.78 ns |    67.707 ns |    63.334 ns |   6,517.67 ns |  0.91 |    0.01 | 1.0910 |     - |     - |    3432 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  45,063.57 ns |   509.636 ns |   476.714 ns |  45,206.88 ns |  0.88 |    0.01 | 0.4272 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  51,038.10 ns |   548.689 ns |   513.244 ns |  51,238.59 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  35,499.41 ns |   496.834 ns |   464.739 ns |  35,605.65 ns |  0.70 |    0.01 | 4.5776 |     - |     - |   14728 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     110.47 ns |     1.158 ns |     1.026 ns |     110.37 ns |  0.74 |    0.01 | 0.0534 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     149.54 ns |     1.946 ns |     1.820 ns |     149.74 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     416.63 ns |     2.519 ns |     1.967 ns |     417.24 ns |  2.78 |    0.04 | 0.1960 |     - |     - |     616 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     202.01 ns |     2.414 ns |     2.258 ns |     202.08 ns |  0.77 |    0.01 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     263.17 ns |     2.403 ns |     2.247 ns |     263.48 ns |  1.00 |    0.00 | 0.1755 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     661.39 ns |     7.135 ns |     6.674 ns |     661.00 ns |  2.51 |    0.04 | 0.2317 |     - |     - |     728 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     790.55 ns |     7.022 ns |     6.569 ns |     792.36 ns |  0.69 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,154.12 ns |    15.409 ns |    14.413 ns |   1,157.12 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,495.29 ns |    18.385 ns |    17.197 ns |   1,499.57 ns |  1.30 |    0.02 | 0.4539 |     - |     - |    1424 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   4,905.04 ns |    62.219 ns |    58.200 ns |   4,907.06 ns |  0.71 |    0.01 | 0.5035 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   6,921.28 ns |    85.034 ns |    79.541 ns |   6,907.05 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   6,323.84 ns |    67.758 ns |    63.381 ns |   6,323.84 ns |  0.91 |    0.01 | 1.1139 |     - |     - |    3496 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  43,104.40 ns |   371.073 ns |   347.102 ns |  43,019.76 ns |  0.87 |    0.01 | 0.4883 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  49,414.86 ns |   478.978 ns |   448.036 ns |  49,198.36 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  34,059.62 ns |   376.817 ns |   352.475 ns |  34,222.74 ns |  0.69 |    0.01 | 4.6387 |     - |     - |   14792 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     103.08 ns |     1.708 ns |     1.598 ns |     104.01 ns |  0.75 |    0.02 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     136.87 ns |     1.902 ns |     1.780 ns |     137.66 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     441.13 ns |     4.874 ns |     4.559 ns |     442.59 ns |  3.22 |    0.05 | 0.1755 |     - |     - |     552 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     193.25 ns |     2.421 ns |     2.265 ns |     193.55 ns |  0.74 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     261.19 ns |     3.539 ns |     3.311 ns |     260.01 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     653.53 ns |     9.413 ns |     8.344 ns |     653.35 ns |  2.50 |    0.04 | 0.2117 |     - |     - |     664 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     757.23 ns |    12.863 ns |    12.032 ns |     757.62 ns |  0.67 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,133.18 ns |    14.286 ns |    13.363 ns |   1,132.05 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,436.50 ns |    14.298 ns |    13.374 ns |   1,437.17 ns |  1.27 |    0.02 | 0.4330 |     - |     - |    1360 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   4,719.74 ns |    66.548 ns |    62.249 ns |   4,714.20 ns |  0.72 |    0.01 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,521.07 ns |    82.103 ns |    76.799 ns |   6,531.31 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   5,478.21 ns |    70.986 ns |    66.401 ns |   5,477.23 ns |  0.84 |    0.02 | 1.0910 |     - |     - |    3432 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  41,363.45 ns |   467.308 ns |   437.120 ns |  41,431.26 ns |  0.87 |    0.02 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  47,598.92 ns |   740.287 ns |   692.465 ns |  47,501.37 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  25,916.85 ns |   296.673 ns |   277.508 ns |  25,936.54 ns |  0.54 |    0.01 | 4.6387 |     - |     - |   14729 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |     101.38 ns |     1.463 ns |     1.368 ns |     101.18 ns |  0.74 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     137.50 ns |     2.318 ns |     2.169 ns |     138.38 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     540.24 ns |     7.252 ns |     6.783 ns |     538.28 ns |  3.93 |    0.08 | 0.1755 |     - |     - |     552 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     505.65 ns |     6.789 ns |     6.350 ns |     507.80 ns |  0.89 |    0.01 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     566.45 ns |     6.563 ns |     6.139 ns |     566.83 ns |  1.00 |    0.00 | 0.1783 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |   1,030.57 ns |     9.164 ns |     8.572 ns |   1,031.29 ns |  1.82 |    0.02 | 0.2117 |     - |     - |     664 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,783.43 ns |    27.786 ns |    25.991 ns |   1,785.65 ns |  0.82 |    0.01 | 0.2518 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,166.77 ns |    18.138 ns |    16.966 ns |   2,173.39 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,639.39 ns |    33.984 ns |    30.126 ns |   2,640.14 ns |  1.22 |    0.02 | 0.4272 |     - |     - |    1360 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  14,002.88 ns |   209.430 ns |   195.901 ns |  13,978.86 ns |  0.88 |    0.02 | 0.5035 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  15,909.24 ns |   271.059 ns |   253.549 ns |  16,029.46 ns |  1.00 |    0.00 | 1.6174 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  14,130.16 ns |   157.881 ns |   147.682 ns |  14,139.48 ns |  0.89 |    0.01 | 1.0834 |     - |     - |    3432 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 128,733.34 ns | 1,698.622 ns | 1,588.892 ns | 128,993.75 ns |  0.96 |    0.02 | 0.4883 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 134,271.89 ns | 2,103.701 ns | 1,967.803 ns | 134,942.80 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 110,031.67 ns | 1,209.321 ns | 1,131.200 ns | 110,530.18 ns |  0.82 |    0.02 | 4.6387 |     - |     - |   14729 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     100.81 ns |     1.521 ns |     1.422 ns |     100.83 ns |  0.70 |    0.01 | 0.0459 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     144.91 ns |     2.697 ns |     2.523 ns |     144.88 ns |  1.00 |    0.00 | 0.1121 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     472.35 ns |     7.661 ns |     7.166 ns |     470.42 ns |  3.26 |    0.09 | 0.1755 |     - |     - |     552 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     198.79 ns |     1.702 ns |     1.592 ns |     198.78 ns |  0.75 |    0.01 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     266.59 ns |     3.299 ns |     3.086 ns |     267.56 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     694.59 ns |     7.164 ns |     6.701 ns |     694.22 ns |  2.61 |    0.03 | 0.2117 |     - |     - |     664 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     761.28 ns |     8.285 ns |     7.750 ns |     764.85 ns |  0.65 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,164.30 ns |    10.830 ns |    10.130 ns |   1,165.80 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,654.11 ns |    21.212 ns |    18.804 ns |   1,660.69 ns |  1.42 |    0.02 | 0.4330 |     - |     - |    1360 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   4,935.11 ns |    60.709 ns |    56.787 ns |   4,938.02 ns |  0.73 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   6,738.64 ns |    56.236 ns |    52.603 ns |   6,716.68 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   6,976.61 ns |   100.190 ns |    93.718 ns |   6,971.59 ns |  1.04 |    0.02 | 1.0910 |     - |     - |    3432 B |
    |             |                |               |               |              |              |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  44,202.80 ns |   395.392 ns |   369.850 ns |  44,223.81 ns |  0.88 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  50,339.86 ns |   545.163 ns |   509.946 ns |  50,350.95 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  37,124.97 ns |   575.107 ns |   537.956 ns |  37,415.88 ns |  0.74 |    0.01 | 4.5776 |     - |     - |   14729 B |    */
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
