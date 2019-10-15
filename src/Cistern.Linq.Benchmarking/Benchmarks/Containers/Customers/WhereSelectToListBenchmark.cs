using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<string>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |         Error |        StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|--------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      15.16 ns |     0.3147 ns |     0.2943 ns |  0.21 |    0.02 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq |          Array |             0 |      78.72 ns |     1.6537 ns |     3.9302 ns |  1.00 |    0.00 | 0.0100 |     - |     - |      32 B |
    | CisternLinq |          Array |             0 |     136.22 ns |     2.0478 ns |     1.9156 ns |  1.87 |    0.13 | 0.0253 |     - |     - |      80 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |      32.95 ns |     0.4031 ns |     0.3574 ns |  0.34 |    0.01 | 0.0204 |     - |     - |      64 B |
    |  SystemLinq |          Array |             1 |      97.64 ns |     1.4555 ns |     1.3615 ns |  1.00 |    0.00 | 0.0432 |     - |     - |     136 B |
    | CisternLinq |          Array |             1 |     190.35 ns |     2.0778 ns |     1.9436 ns |  1.95 |    0.03 | 0.0508 |     - |     - |     160 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     206.57 ns |     3.3353 ns |     2.9567 ns |  0.73 |    0.01 | 0.0663 |     - |     - |     208 B |
    |  SystemLinq |          Array |            10 |     282.89 ns |     2.5445 ns |     2.3801 ns |  1.00 |    0.00 | 0.0892 |     - |     - |     280 B |
    | CisternLinq |          Array |            10 |     358.54 ns |     5.8399 ns |     5.4626 ns |  1.27 |    0.02 | 0.0963 |     - |     - |     304 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   2,122.97 ns |    30.9000 ns |    25.8029 ns |  0.89 |    0.01 | 0.7057 |     - |     - |    2224 B |
    |  SystemLinq |          Array |           100 |   2,376.40 ns |    47.5291 ns |    44.4587 ns |  1.00 |    0.00 | 0.7286 |     - |     - |    2296 B |
    | CisternLinq |          Array |           100 |   2,347.37 ns |    46.8015 ns |    96.6533 ns |  1.00 |    0.05 | 0.7362 |     - |     - |    2320 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  23,860.90 ns |   178.1392 ns |   148.7543 ns |  1.10 |    0.01 | 5.2795 |     - |     - |   16632 B |
    |  SystemLinq |          Array |          1000 |  21,806.28 ns |   215.4368 ns |   201.5197 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16704 B |
    | CisternLinq |          Array |          1000 |  21,881.29 ns |   181.3903 ns |   141.6177 ns |  1.00 |    0.01 | 5.3101 |     - |     - |   16728 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |      23.65 ns |     0.1958 ns |     0.1635 ns |  0.26 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |           List |             0 |      91.39 ns |     1.5728 ns |     1.4712 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq |           List |             0 |     176.43 ns |     1.3729 ns |     1.2842 ns |  1.93 |    0.03 | 0.0663 |     - |     - |     208 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |      40.12 ns |     0.5898 ns |     0.5229 ns |  0.39 |    0.01 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |           List |             1 |     102.24 ns |     1.5259 ns |     1.4273 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq |           List |             1 |     190.04 ns |     2.4739 ns |     2.3140 ns |  1.86 |    0.03 | 0.0660 |     - |     - |     208 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     264.04 ns |     1.0588 ns |     0.8267 ns |  0.91 |    0.01 | 0.0682 |     - |     - |     216 B |
    |  SystemLinq |           List |            10 |     288.08 ns |     3.2648 ns |     3.0539 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |           List |            10 |     360.26 ns |     4.8060 ns |     4.4956 ns |  1.25 |    0.02 | 0.1121 |     - |     - |     352 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   2,699.13 ns |    37.7229 ns |    35.2860 ns |  1.28 |    0.02 | 0.7095 |     - |     - |    2232 B |
    |  SystemLinq |           List |           100 |   2,108.39 ns |    38.0720 ns |    31.7919 ns |  1.00 |    0.00 | 0.7439 |     - |     - |    2344 B |
    | CisternLinq |           List |           100 |   2,147.53 ns |    33.6422 ns |    31.4690 ns |  1.02 |    0.02 | 0.7515 |     - |     - |    2368 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  28,475.01 ns |   296.7201 ns |   277.5521 ns |  1.26 |    0.02 | 5.2795 |     - |     - |   16640 B |
    |  SystemLinq |           List |          1000 |  22,688.62 ns |   280.8025 ns |   262.6628 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16752 B |
    | CisternLinq |           List |          1000 |  22,757.02 ns |   292.4742 ns |   273.5805 ns |  1.00 |    0.02 | 5.3406 |     - |     - |   16776 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |      27.50 ns |     0.7702 ns |     0.7204 ns |  0.25 |    0.01 | 0.0306 |     - |     - |      96 B |
    |  SystemLinq |     Enumerable |             0 |     112.08 ns |     2.2984 ns |     2.6469 ns |  1.00 |    0.00 | 0.0688 |     - |     - |     216 B |
    | CisternLinq |     Enumerable |             0 |     240.10 ns |     3.6919 ns |     3.4534 ns |  2.16 |    0.06 | 0.0758 |     - |     - |     240 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |      44.70 ns |     0.6137 ns |     0.5741 ns |  0.34 |    0.01 | 0.0306 |     - |     - |      96 B |
    |  SystemLinq |     Enumerable |             1 |     129.97 ns |     1.6579 ns |     1.5508 ns |  1.00 |    0.00 | 0.0687 |     - |     - |     216 B |
    | CisternLinq |     Enumerable |             1 |     252.45 ns |     3.5835 ns |     3.3520 ns |  1.94 |    0.04 | 0.0763 |     - |     - |     240 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     235.61 ns |     3.5798 ns |     3.3485 ns |  0.64 |    0.01 | 0.0763 |     - |     - |     240 B |
    |  SystemLinq |     Enumerable |            10 |     367.87 ns |     4.2586 ns |     3.9835 ns |  1.00 |    0.00 | 0.1140 |     - |     - |     360 B |
    | CisternLinq |     Enumerable |            10 |     481.40 ns |     5.4216 ns |     5.0713 ns |  1.31 |    0.02 | 0.1221 |     - |     - |     384 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   2,434.48 ns |    28.6146 ns |    26.7661 ns |  0.83 |    0.01 | 0.7172 |     - |     - |    2256 B |
    |  SystemLinq |     Enumerable |           100 |   2,949.02 ns |    30.7484 ns |    28.7621 ns |  1.00 |    0.00 | 0.7553 |     - |     - |    2376 B |
    | CisternLinq |     Enumerable |           100 |   3,103.34 ns |    45.7816 ns |    42.8242 ns |  1.05 |    0.02 | 0.7629 |     - |     - |    2400 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  26,191.22 ns |   217.5493 ns |   203.4957 ns |  0.88 |    0.01 | 5.2795 |     - |     - |   16664 B |
    |  SystemLinq |     Enumerable |          1000 |  29,829.32 ns |   393.3017 ns |   367.8946 ns |  1.00 |    0.00 | 5.3406 |     - |     - |   16784 B |
    | CisternLinq |     Enumerable |          1000 |  30,750.76 ns |   494.4122 ns |   462.4735 ns |  1.03 |    0.02 | 5.3406 |     - |     - |   16808 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |      20.95 ns |     0.4938 ns |     0.5878 ns |  0.19 |    0.00 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq | ImmutableArray |             0 |     108.19 ns |     1.7595 ns |     1.6459 ns |  1.00 |    0.00 | 0.0484 |     - |     - |     152 B |
    | CisternLinq | ImmutableArray |             0 |     264.89 ns |     3.1925 ns |     2.9862 ns |  2.45 |    0.05 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |      39.34 ns |     0.5732 ns |     0.5362 ns |  0.32 |    0.01 | 0.0203 |     - |     - |      64 B |
    |  SystemLinq | ImmutableArray |             1 |     124.21 ns |     1.7989 ns |     1.6827 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq | ImmutableArray |             1 |     278.04 ns |     3.3792 ns |     3.1609 ns |  2.24 |    0.04 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     215.93 ns |     2.9024 ns |     2.7149 ns |  0.64 |    0.01 | 0.0663 |     - |     - |     208 B |
    |  SystemLinq | ImmutableArray |            10 |     338.58 ns |     4.3482 ns |     4.0673 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq | ImmutableArray |            10 |     459.63 ns |     5.0284 ns |     4.7036 ns |  1.36 |    0.02 | 0.1063 |     - |     - |     336 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   2,147.09 ns |    19.6186 ns |    18.3512 ns |  0.82 |    0.01 | 0.7057 |     - |     - |    2224 B |
    |  SystemLinq | ImmutableArray |           100 |   2,633.16 ns |    37.5984 ns |    31.3963 ns |  1.00 |    0.00 | 0.7439 |     - |     - |    2344 B |
    | CisternLinq | ImmutableArray |           100 |   2,261.64 ns |    30.1983 ns |    28.2475 ns |  0.86 |    0.02 | 0.7477 |     - |     - |    2352 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  24,529.31 ns |   285.0120 ns |   266.6004 ns |  0.93 |    0.01 | 5.2795 |     - |     - |   16632 B |
    |  SystemLinq | ImmutableArray |          1000 |  26,497.08 ns |   278.2300 ns |   260.2565 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16752 B |
    | CisternLinq | ImmutableArray |          1000 |  22,022.30 ns |   276.6694 ns |   258.7967 ns |  0.83 |    0.01 | 5.3406 |     - |     - |   16762 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      18.85 ns |     0.1703 ns |     0.1510 ns |  0.18 |    0.00 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq |  ImmutableList |             0 |     101.95 ns |     1.3792 ns |     1.2901 ns |  1.00 |    0.00 | 0.0484 |     - |     - |     152 B |
    | CisternLinq |  ImmutableList |             0 |     350.13 ns |     2.6408 ns |     2.4702 ns |  3.43 |    0.04 | 0.0863 |     - |     - |     272 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     313.25 ns |     2.8359 ns |     2.6527 ns |  0.72 |    0.01 | 0.0329 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             1 |     433.08 ns |     0.7267 ns |     0.6797 ns |  1.00 |    0.00 | 0.0710 |     - |     - |     224 B |
    | CisternLinq |  ImmutableList |             1 |     617.98 ns |     5.6109 ns |     5.2485 ns |  1.43 |    0.01 | 0.0858 |     - |     - |     272 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,277.35 ns |    13.6008 ns |    12.7222 ns |  0.94 |    0.01 | 0.0763 |     - |     - |     248 B |
    |  SystemLinq |  ImmutableList |            10 |   1,365.18 ns |    12.9974 ns |    12.1578 ns |  1.00 |    0.00 | 0.1144 |     - |     - |     368 B |
    | CisternLinq |  ImmutableList |            10 |   1,623.67 ns |    21.4103 ns |    20.0272 ns |  1.19 |    0.02 | 0.1297 |     - |     - |     416 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |   9,898.59 ns |   135.5225 ns |   126.7678 ns |  0.94 |    0.02 | 0.7172 |     - |     - |    2264 B |
    |  SystemLinq |  ImmutableList |           100 |  10,545.31 ns |   128.1388 ns |   119.8611 ns |  1.00 |    0.00 | 0.7477 |     - |     - |    2384 B |
    | CisternLinq |  ImmutableList |           100 |  10,727.18 ns |    94.2869 ns |    83.5829 ns |  1.02 |    0.01 | 0.7629 |     - |     - |    2432 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 103,385.54 ns | 1,332.6589 ns | 1,246.5699 ns |  0.95 |    0.02 | 5.2490 |     - |     - |   16672 B |
    |  SystemLinq |  ImmutableList |          1000 | 109,099.34 ns | 1,754.8114 ns | 1,641.4517 ns |  1.00 |    0.00 | 5.2490 |     - |     - |   16792 B |
    | CisternLinq |  ImmutableList |          1000 | 101,388.58 ns |   350.7334 ns |   292.8783 ns |  0.93 |    0.01 | 5.2490 |     - |     - |   16842 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |      20.62 ns |     0.3471 ns |     0.3077 ns |  0.19 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |     FSharpList |             0 |     107.39 ns |     1.5442 ns |     1.4444 ns |  1.00 |    0.00 | 0.0612 |     - |     - |     192 B |
    | CisternLinq |     FSharpList |             0 |     254.62 ns |     2.7609 ns |     2.5826 ns |  2.37 |    0.03 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |      38.29 ns |     0.5623 ns |     0.5260 ns |  0.31 |    0.01 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |     FSharpList |             1 |     125.33 ns |     1.8057 ns |     1.6891 ns |  1.00 |    0.00 | 0.0610 |     - |     - |     192 B |
    | CisternLinq |     FSharpList |             1 |     278.81 ns |     2.6152 ns |     2.4462 ns |  2.23 |    0.03 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     230.87 ns |     3.3734 ns |     3.1555 ns |  0.67 |    0.01 | 0.0687 |     - |     - |     216 B |
    |  SystemLinq |     FSharpList |            10 |     343.44 ns |     4.7533 ns |     4.4463 ns |  1.00 |    0.00 | 0.1068 |     - |     - |     336 B |
    | CisternLinq |     FSharpList |            10 |     558.74 ns |     6.5342 ns |     6.1121 ns |  1.63 |    0.03 | 0.1059 |     - |     - |     336 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   2,385.36 ns |    26.8240 ns |    25.0912 ns |  0.81 |    0.01 | 0.7095 |     - |     - |    2232 B |
    |  SystemLinq |     FSharpList |           100 |   2,950.02 ns |    18.8799 ns |    17.6603 ns |  1.00 |    0.00 | 0.7477 |     - |     - |    2352 B |
    | CisternLinq |     FSharpList |           100 |   3,339.06 ns |    36.3016 ns |    33.9566 ns |  1.13 |    0.02 | 0.7477 |     - |     - |    2352 B |
    |             |                |               |               |               |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  26,627.74 ns |   489.6462 ns |   458.0154 ns |  0.87 |    0.02 | 5.2795 |     - |     - |   16640 B |
    |  SystemLinq |     FSharpList |          1000 |  30,740.36 ns |   389.9793 ns |   364.7869 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16760 B |
    | CisternLinq |     FSharpList |          1000 |  30,342.46 ns |   530.1101 ns |   495.8654 ns |  0.99 |    0.02 | 5.3406 |     - |     - |   16762 B |*/
    [CoreJob, MemoryDiagnoser]
    public class Containers_SelectWhereToListBenchmark : CustomersBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var subset = new List<string>();
            foreach (var c in Customers)
            {
                if (c.DOB.Year < 1980)
                {
                    subset.Add(c.Name);
                }
            }
            return subset;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.Select(
                        System.Linq.Enumerable.Where(
                            Customers,
                            c => c.DOB.Year < 1980),
                        c => c.Name)
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return 
                Customers
                .Where(c => c.DOB.Year < 1980)
                .Select(c => c.Name)
                .ToList();
        }

    }
}
