using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      95.36 ns |     1.1000 ns |     1.0290 ns |  0.71 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     134.60 ns |     1.6661 ns |     1.5585 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     235.44 ns |     4.0465 ns |     3.7851 ns |  1.75 |    0.03 | 0.1605 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     184.74 ns |     1.6627 ns |     1.5553 ns |  0.72 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     256.44 ns |     3.7531 ns |     3.5107 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     442.08 ns |     5.6524 ns |     5.2873 ns |  1.72 |    0.02 | 0.1960 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     749.36 ns |    10.4791 ns |     9.2895 ns |  0.67 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,120.39 ns |    11.5087 ns |    10.7652 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,211.84 ns |     7.8800 ns |     6.9854 ns |  1.08 |    0.01 | 0.4177 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   4,650.14 ns |    51.6323 ns |    48.2969 ns |  0.71 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   6,534.50 ns |    71.7159 ns |    67.0831 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   4,985.15 ns |    49.1387 ns |    45.9644 ns |  0.76 |    0.01 | 1.0757 |     - |     - |    3384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  41,232.51 ns |   497.9956 ns |   465.8254 ns |  0.87 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  47,569.93 ns |   680.2502 ns |   636.3065 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  24,104.51 ns |   118.3186 ns |   110.6753 ns |  0.51 |    0.01 | 4.6387 |     - |     - |   14680 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     110.63 ns |     1.4058 ns |     1.3150 ns |  0.78 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     142.11 ns |     1.2501 ns |     1.0439 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     239.38 ns |     2.7170 ns |     2.5415 ns |  1.69 |    0.02 | 0.1602 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     197.98 ns |     1.3611 ns |     1.2732 ns |  0.75 |    0.01 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     264.06 ns |     2.8229 ns |     2.6406 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     474.64 ns |     5.4456 ns |     5.0938 ns |  1.80 |    0.01 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     794.47 ns |     9.7458 ns |     9.1163 ns |  0.66 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,206.21 ns |    14.9958 ns |    13.2934 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,279.54 ns |    14.4931 ns |    13.5569 ns |  1.06 |    0.02 | 0.4177 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,080.14 ns |    66.9036 ns |    62.5817 ns |  0.74 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   6,892.27 ns |    84.5546 ns |    79.0925 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   6,173.81 ns |    66.8734 ns |    62.5535 ns |  0.90 |    0.01 | 1.0681 |     - |     - |    3384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  45,048.97 ns |   444.8143 ns |   416.0796 ns |  0.89 |    0.01 | 0.4272 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  50,822.34 ns |   459.7626 ns |   430.0622 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  32,977.26 ns |   428.9172 ns |   401.2094 ns |  0.65 |    0.01 | 4.6387 |     - |     - |   14680 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     110.29 ns |     2.3254 ns |     3.4086 ns |  0.76 |    0.03 | 0.0534 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     145.86 ns |     1.4713 ns |     1.3043 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     336.19 ns |     4.2037 ns |     3.9321 ns |  2.31 |    0.03 | 0.1807 |     - |     - |     568 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     202.80 ns |     3.9420 ns |     3.2918 ns |  0.75 |    0.02 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     270.97 ns |     5.1618 ns |     5.3008 ns |  1.00 |    0.00 | 0.1760 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     525.53 ns |     5.4503 ns |     5.0982 ns |  1.94 |    0.04 | 0.2155 |     - |     - |     680 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     783.15 ns |     8.3033 ns |     7.7669 ns |  0.69 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,138.94 ns |    17.2586 ns |    16.1437 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,342.26 ns |    16.7463 ns |    15.6645 ns |  1.18 |    0.01 | 0.4387 |     - |     - |    1376 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   4,904.26 ns |    58.5595 ns |    54.7766 ns |  0.72 |    0.01 | 0.4959 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   6,765.68 ns |    80.4522 ns |    75.2550 ns |  1.00 |    0.00 | 1.6632 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   5,964.87 ns |    54.7534 ns |    51.2163 ns |  0.88 |    0.01 | 1.0986 |     - |     - |    3448 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  43,274.68 ns |   388.6259 ns |   363.5209 ns |  0.88 |    0.01 | 0.4883 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  49,153.87 ns |   539.7266 ns |   504.8606 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  32,098.92 ns |   326.7146 ns |   305.6090 ns |  0.65 |    0.01 | 4.6997 |     - |     - |   14744 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     103.32 ns |     2.1337 ns |     2.7744 ns |  0.73 |    0.03 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     142.60 ns |     2.1349 ns |     1.9970 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     347.64 ns |     3.5233 ns |     3.2957 ns |  2.44 |    0.04 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     190.00 ns |     3.0423 ns |     2.8458 ns |  0.74 |    0.02 | 0.1147 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     255.43 ns |     3.1205 ns |     2.9189 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     548.41 ns |     5.6546 ns |     5.2893 ns |  2.15 |    0.03 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     748.44 ns |     8.8799 ns |     8.3063 ns |  0.67 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,122.99 ns |    12.7013 ns |    11.8808 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,320.26 ns |    16.4145 ns |    15.3541 ns |  1.18 |    0.02 | 0.4177 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   4,691.24 ns |    37.7416 ns |    35.3035 ns |  0.72 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   6,536.07 ns |    99.7597 ns |    93.3153 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   5,257.45 ns |    55.0858 ns |    51.5273 ns |  0.80 |    0.01 | 1.0681 |     - |     - |    3384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  41,817.82 ns |   467.3534 ns |   437.1627 ns |  0.88 |    0.01 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  47,721.89 ns |   548.6242 ns |   513.1834 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  24,207.42 ns |   227.9145 ns |   213.1914 ns |  0.51 |    0.01 | 4.6692 |     - |     - |   14681 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      99.52 ns |     0.5639 ns |     0.5275 ns |  0.73 |    0.01 | 0.0331 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     136.59 ns |     2.1676 ns |     2.0276 ns |  1.00 |    0.00 | 0.0994 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     453.08 ns |     5.6194 ns |     5.2564 ns |  3.32 |    0.06 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     493.05 ns |     7.0388 ns |     6.5841 ns |  0.89 |    0.02 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     554.92 ns |     6.8300 ns |     6.3888 ns |  1.00 |    0.00 | 0.1783 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |     918.08 ns |    13.6764 ns |    12.7929 ns |  1.65 |    0.03 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,783.11 ns |    23.7348 ns |    22.2015 ns |  0.82 |    0.01 | 0.2537 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,186.77 ns |    26.2899 ns |    24.5916 ns |  1.00 |    0.00 | 0.4311 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,471.71 ns |    28.7270 ns |    26.8713 ns |  1.13 |    0.02 | 0.4120 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  13,474.69 ns |   177.6911 ns |   166.2124 ns |  0.85 |    0.01 | 0.5035 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  15,888.16 ns |   183.3977 ns |   171.5504 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  13,595.59 ns |   171.6980 ns |   160.6064 ns |  0.86 |    0.01 | 1.0681 |     - |     - |    3384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 131,991.38 ns | 1,621.1042 ns | 1,516.3819 ns |  0.96 |    0.01 | 0.2441 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 137,434.78 ns |   808.8088 ns |   716.9880 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 105,951.49 ns | 1,096.2446 ns | 1,025.4279 ns |  0.77 |    0.01 | 4.6387 |     - |     - |   14681 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     101.82 ns |     1.0825 ns |     0.9596 ns |  0.72 |    0.01 | 0.0458 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     142.03 ns |     1.6516 ns |     1.5449 ns |  1.00 |    0.00 | 0.1121 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     374.68 ns |     3.8744 ns |     3.6241 ns |  2.64 |    0.04 | 0.1607 |     - |     - |     504 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     191.69 ns |     2.3765 ns |     2.2230 ns |  0.73 |    0.02 | 0.1171 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     263.55 ns |     4.4715 ns |     4.1827 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     582.94 ns |     8.2838 ns |     7.7486 ns |  2.21 |    0.05 | 0.1955 |     - |     - |     616 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     763.09 ns |     7.9919 ns |     7.4756 ns |  0.67 |    0.01 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,139.65 ns |    12.1336 ns |    11.3498 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,446.34 ns |    13.8437 ns |    12.9494 ns |  1.27 |    0.02 | 0.4158 |     - |     - |    1312 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   4,820.58 ns |    60.8936 ns |    56.9599 ns |  0.72 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   6,665.57 ns |    78.3824 ns |    73.3190 ns |  1.00 |    0.00 | 1.6556 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   6,492.73 ns |    84.9874 ns |    79.4973 ns |  0.97 |    0.01 | 1.0757 |     - |     - |    3384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  43,933.79 ns |   607.1132 ns |   567.8940 ns |  0.89 |    0.02 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  49,573.74 ns |   722.6958 ns |   676.0101 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  34,247.04 ns |   456.7962 ns |   427.2875 ns |  0.69 |    0.01 | 4.5776 |     - |     - |   14681 B |
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
