using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<string>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |          Mean |       Error |      StdDev |        Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |--------------:|------------:|------------:|--------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |      18.07 ns |   0.2902 ns |   0.2714 ns |      17.96 ns |  0.20 |    0.00 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq |          Array |             0 |      88.81 ns |   1.5548 ns |   1.4544 ns |      89.17 ns |  1.00 |    0.00 | 0.0101 |     - |     - |      32 B |
    | CisternLinq |          Array |             0 |     155.78 ns |   1.2337 ns |   1.1540 ns |     155.66 ns |  1.75 |    0.03 | 0.0253 |     - |     - |      80 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |      37.54 ns |   0.2871 ns |   0.2685 ns |      37.61 ns |  0.31 |    0.00 | 0.0203 |     - |     - |      64 B |
    |  SystemLinq |          Array |             1 |     120.18 ns |   0.9686 ns |   0.9060 ns |     120.34 ns |  1.00 |    0.00 | 0.0432 |     - |     - |     136 B |
    | CisternLinq |          Array |             1 |     205.74 ns |   1.5913 ns |   1.4107 ns |     205.75 ns |  1.71 |    0.02 | 0.0508 |     - |     - |     160 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     233.31 ns |   2.2810 ns |   2.1337 ns |     232.62 ns |  0.75 |    0.01 | 0.0658 |     - |     - |     208 B |
    |  SystemLinq |          Array |            10 |     309.87 ns |   3.2888 ns |   3.0764 ns |     310.50 ns |  1.00 |    0.00 | 0.0892 |     - |     - |     280 B |
    | CisternLinq |          Array |            10 |     405.38 ns |   1.6825 ns |   1.4050 ns |     405.24 ns |  1.31 |    0.01 | 0.0968 |     - |     - |     304 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   2,401.35 ns |  16.4341 ns |  15.3725 ns |   2,399.53 ns |  1.06 |    0.01 | 0.7057 |     - |     - |    2224 B |
    |  SystemLinq |          Array |           100 |   2,265.68 ns |  12.5767 ns |  11.7643 ns |   2,268.35 ns |  1.00 |    0.00 | 0.7286 |     - |     - |    2296 B |
    | CisternLinq |          Array |           100 |   2,351.84 ns |  15.1609 ns |  14.1816 ns |   2,352.29 ns |  1.04 |    0.01 | 0.7362 |     - |     - |    2320 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  27,879.06 ns | 530.2006 ns | 631.1661 ns |  28,028.33 ns |  1.11 |    0.04 | 5.2795 |     - |     - |   16632 B |
    |  SystemLinq |          Array |          1000 |  25,004.85 ns | 418.0022 ns | 390.9995 ns |  25,103.95 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16704 B |
    | CisternLinq |          Array |          1000 |  24,576.22 ns | 199.8788 ns | 177.1873 ns |  24,499.06 ns |  0.98 |    0.02 | 5.3101 |     - |     - |   16728 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |      28.04 ns |   0.2647 ns |   0.2476 ns |      28.10 ns |  0.27 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |           List |             0 |     105.00 ns |   0.9038 ns |   0.8454 ns |     104.95 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq |           List |             0 |     188.75 ns |   0.8969 ns |   0.8390 ns |     188.81 ns |  1.80 |    0.02 | 0.0660 |     - |     - |     208 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |      44.52 ns |   0.2505 ns |   0.2221 ns |      44.55 ns |  0.38 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |           List |             1 |     116.03 ns |   0.9705 ns |   0.9079 ns |     116.30 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq |           List |             1 |     210.55 ns |   3.8956 ns |   3.6439 ns |     209.88 ns |  1.81 |    0.03 | 0.0660 |     - |     - |     208 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     284.57 ns |   2.1903 ns |   2.0488 ns |     284.35 ns |  0.88 |    0.01 | 0.0682 |     - |     - |     216 B |
    |  SystemLinq |           List |            10 |     322.59 ns |   2.1633 ns |   2.0235 ns |     322.93 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq |           List |            10 |     399.05 ns |   2.1014 ns |   1.9657 ns |     399.49 ns |  1.24 |    0.01 | 0.1116 |     - |     - |     352 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   3,010.14 ns |  21.2850 ns |  19.9100 ns |   3,009.39 ns |  1.27 |    0.01 | 0.7095 |     - |     - |    2232 B |
    |  SystemLinq |           List |           100 |   2,364.51 ns |  22.1177 ns |  20.6889 ns |   2,369.64 ns |  1.00 |    0.00 | 0.7439 |     - |     - |    2344 B |
    | CisternLinq |           List |           100 |   2,395.19 ns |  11.2521 ns |  10.5252 ns |   2,395.83 ns |  1.01 |    0.01 | 0.7515 |     - |     - |    2368 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  31,685.41 ns | 196.0240 ns | 183.3610 ns |  31,737.89 ns |  1.23 |    0.01 | 5.2490 |     - |     - |   16640 B |
    |  SystemLinq |           List |          1000 |  25,648.59 ns | 118.8395 ns | 105.3482 ns |  25,666.48 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16752 B |
    | CisternLinq |           List |          1000 |  25,386.79 ns | 115.1738 ns | 107.7337 ns |  25,409.65 ns |  0.99 |    0.01 | 5.3406 |     - |     - |   16776 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |      32.82 ns |   0.2714 ns |   0.2406 ns |      32.91 ns |  0.26 |    0.00 | 0.0305 |     - |     - |      96 B |
    |  SystemLinq |     Enumerable |             0 |     127.11 ns |   2.1050 ns |   1.7578 ns |     126.74 ns |  1.00 |    0.00 | 0.0687 |     - |     - |     216 B |
    | CisternLinq |     Enumerable |             0 |     256.95 ns |   2.4084 ns |   2.2528 ns |     257.40 ns |  2.02 |    0.04 | 0.0758 |     - |     - |     240 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |      51.92 ns |   0.2752 ns |   0.2574 ns |      51.90 ns |  0.35 |    0.00 | 0.0305 |     - |     - |      96 B |
    |  SystemLinq |     Enumerable |             1 |     147.50 ns |   1.3004 ns |   1.2164 ns |     147.43 ns |  1.00 |    0.00 | 0.0687 |     - |     - |     216 B |
    | CisternLinq |     Enumerable |             1 |     296.02 ns |   1.8701 ns |   1.7493 ns |     296.14 ns |  2.01 |    0.02 | 0.0763 |     - |     - |     240 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     268.05 ns |   1.9968 ns |   1.8678 ns |     268.21 ns |  0.64 |    0.01 | 0.0763 |     - |     - |     240 B |
    |  SystemLinq |     Enumerable |            10 |     421.39 ns |   2.2527 ns |   2.1072 ns |     421.49 ns |  1.00 |    0.00 | 0.1140 |     - |     - |     360 B |
    | CisternLinq |     Enumerable |            10 |     536.16 ns |   2.0669 ns |   1.8323 ns |     535.95 ns |  1.27 |    0.01 | 0.1221 |     - |     - |     384 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   2,729.50 ns |  15.4117 ns |  14.4161 ns |   2,733.20 ns |  0.83 |    0.01 | 0.7172 |     - |     - |    2256 B |
    |  SystemLinq |     Enumerable |           100 |   3,275.72 ns |  21.8108 ns |  20.4018 ns |   3,274.36 ns |  1.00 |    0.00 | 0.7553 |     - |     - |    2376 B |
    | CisternLinq |     Enumerable |           100 |   3,475.75 ns |  23.4703 ns |  21.9541 ns |   3,480.20 ns |  1.06 |    0.01 | 0.7629 |     - |     - |    2400 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  30,153.23 ns | 264.3488 ns | 247.2720 ns |  30,148.17 ns |  0.91 |    0.01 | 5.2795 |     - |     - |   16664 B |
    |  SystemLinq |     Enumerable |          1000 |  33,232.80 ns | 254.2772 ns | 237.8510 ns |  33,211.13 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16784 B |
    | CisternLinq |     Enumerable |          1000 |  34,370.98 ns | 262.8108 ns | 245.8334 ns |  34,418.26 ns |  1.03 |    0.01 | 5.3101 |     - |     - |   16808 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |      22.84 ns |   0.2013 ns |   0.1883 ns |      22.82 ns |  0.19 |    0.01 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq | ImmutableArray |             0 |     119.20 ns |   2.4667 ns |   3.3765 ns |     117.02 ns |  1.00 |    0.00 | 0.0484 |     - |     - |     152 B |
    | CisternLinq | ImmutableArray |             0 |     287.07 ns |   1.6921 ns |   1.5827 ns |     286.74 ns |  2.38 |    0.07 | 0.0606 |     - |     - |     192 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |      45.49 ns |   0.4247 ns |   0.3973 ns |      45.58 ns |  0.32 |    0.00 | 0.0203 |     - |     - |      64 B |
    |  SystemLinq | ImmutableArray |             1 |     141.52 ns |   1.0674 ns |   0.9984 ns |     141.55 ns |  1.00 |    0.00 | 0.0587 |     - |     - |     184 B |
    | CisternLinq | ImmutableArray |             1 |     300.72 ns |   3.0258 ns |   2.8303 ns |     300.52 ns |  2.12 |    0.02 | 0.0606 |     - |     - |     192 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     244.66 ns |   2.1225 ns |   1.9854 ns |     244.79 ns |  0.64 |    0.01 | 0.0663 |     - |     - |     208 B |
    |  SystemLinq | ImmutableArray |            10 |     381.55 ns |   2.9371 ns |   2.7474 ns |     381.37 ns |  1.00 |    0.00 | 0.1044 |     - |     - |     328 B |
    | CisternLinq | ImmutableArray |            10 |     503.03 ns |   2.8974 ns |   2.7102 ns |     503.30 ns |  1.32 |    0.01 | 0.1059 |     - |     - |     336 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   2,416.14 ns |  26.9434 ns |  23.8846 ns |   2,414.53 ns |  0.81 |    0.01 | 0.7057 |     - |     - |    2224 B |
    |  SystemLinq | ImmutableArray |           100 |   2,965.43 ns |  18.5242 ns |  17.3276 ns |   2,966.26 ns |  1.00 |    0.00 | 0.7439 |     - |     - |    2344 B |
    | CisternLinq | ImmutableArray |           100 |   2,561.33 ns |  11.0562 ns |   9.8011 ns |   2,562.60 ns |  0.86 |    0.00 | 0.7477 |     - |     - |    2352 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  27,336.08 ns | 198.7519 ns | 185.9127 ns |  27,340.74 ns |  0.90 |    0.01 | 5.2795 |     - |     - |   16632 B |
    |  SystemLinq | ImmutableArray |          1000 |  30,366.32 ns | 237.2682 ns | 221.9408 ns |  30,441.77 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16752 B |
    | CisternLinq | ImmutableArray |          1000 |  25,176.45 ns | 185.8699 ns | 173.8628 ns |  25,181.21 ns |  0.83 |    0.01 | 5.3406 |     - |     - |   16762 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |      22.50 ns |   0.5418 ns |   0.5068 ns |      22.67 ns |  0.19 |    0.00 | 0.0102 |     - |     - |      32 B |
    |  SystemLinq |  ImmutableList |             0 |     119.08 ns |   0.8952 ns |   0.7475 ns |     119.30 ns |  1.00 |    0.00 | 0.0482 |     - |     - |     152 B |
    | CisternLinq |  ImmutableList |             0 |     383.76 ns |   2.3434 ns |   2.1920 ns |     383.61 ns |  3.22 |    0.03 | 0.0863 |     - |     - |     272 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     366.36 ns |   1.7592 ns |   1.6456 ns |     366.89 ns |  0.76 |    0.01 | 0.0329 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             1 |     483.92 ns |   3.8298 ns |   3.5824 ns |     483.29 ns |  1.00 |    0.00 | 0.0706 |     - |     - |     224 B |
    | CisternLinq |  ImmutableList |             1 |     667.87 ns |   3.7432 ns |   3.5014 ns |     667.98 ns |  1.38 |    0.01 | 0.0858 |     - |     - |     272 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   1,396.30 ns |   8.7280 ns |   8.1641 ns |   1,396.82 ns |  0.88 |    0.01 | 0.0763 |     - |     - |     248 B |
    |  SystemLinq |  ImmutableList |            10 |   1,590.89 ns |   8.5962 ns |   8.0409 ns |   1,592.25 ns |  1.00 |    0.00 | 0.1144 |     - |     - |     368 B |
    | CisternLinq |  ImmutableList |            10 |   1,810.80 ns |  10.6438 ns |   9.9563 ns |   1,811.79 ns |  1.14 |    0.01 | 0.1297 |     - |     - |     416 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  11,209.85 ns |  87.4833 ns |  81.8319 ns |  11,242.20 ns |  0.93 |    0.01 | 0.7172 |     - |     - |    2264 B |
    |  SystemLinq |  ImmutableList |           100 |  12,020.09 ns |  56.2063 ns |  52.5754 ns |  12,014.03 ns |  1.00 |    0.00 | 0.7477 |     - |     - |    2384 B |
    | CisternLinq |  ImmutableList |           100 |  12,256.57 ns |  69.2535 ns |  64.7798 ns |  12,276.27 ns |  1.02 |    0.01 | 0.7629 |     - |     - |    2432 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 116,509.18 ns | 444.2295 ns | 415.5325 ns | 116,372.69 ns |  0.97 |    0.01 | 5.2490 |     - |     - |   16672 B |
    |  SystemLinq |  ImmutableList |          1000 | 120,611.84 ns | 528.2532 ns | 468.2827 ns | 120,694.46 ns |  1.00 |    0.00 | 5.2490 |     - |     - |   16792 B |
    | CisternLinq |  ImmutableList |          1000 | 121,727.51 ns | 717.1535 ns | 670.8258 ns | 121,912.71 ns |  1.01 |    0.01 | 5.2490 |     - |     - |   16842 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |      24.30 ns |   0.1464 ns |   0.1370 ns |      24.28 ns |  0.19 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |     FSharpList |             0 |     124.82 ns |   0.6832 ns |   0.6391 ns |     124.79 ns |  1.00 |    0.00 | 0.0610 |     - |     - |     192 B |
    | CisternLinq |     FSharpList |             0 |     278.98 ns |   1.8654 ns |   1.7449 ns |     279.41 ns |  2.24 |    0.02 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |      43.78 ns |   0.3007 ns |   0.2813 ns |      43.80 ns |  0.32 |    0.00 | 0.0229 |     - |     - |      72 B |
    |  SystemLinq |     FSharpList |             1 |     135.50 ns |   0.8327 ns |   0.7789 ns |     135.67 ns |  1.00 |    0.00 | 0.0608 |     - |     - |     192 B |
    | CisternLinq |     FSharpList |             1 |     304.42 ns |   2.3178 ns |   2.1681 ns |     304.79 ns |  2.25 |    0.02 | 0.0610 |     - |     - |     192 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     261.05 ns |   1.7020 ns |   1.4213 ns |     261.47 ns |  0.67 |    0.01 | 0.0687 |     - |     - |     216 B |
    |  SystemLinq |     FSharpList |            10 |     392.51 ns |   2.4693 ns |   2.3098 ns |     393.06 ns |  1.00 |    0.00 | 0.1068 |     - |     - |     336 B |
    | CisternLinq |     FSharpList |            10 |     624.39 ns |   4.3235 ns |   4.0442 ns |     625.84 ns |  1.59 |    0.01 | 0.1059 |     - |     - |     336 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   2,686.01 ns |  13.3112 ns |  11.8000 ns |   2,685.65 ns |  0.81 |    0.00 | 0.7095 |     - |     - |    2232 B |
    |  SystemLinq |     FSharpList |           100 |   3,298.13 ns |  17.6180 ns |  16.4799 ns |   3,296.45 ns |  1.00 |    0.00 | 0.7477 |     - |     - |    2352 B |
    | CisternLinq |     FSharpList |           100 |   3,677.55 ns |  24.7260 ns |  23.1287 ns |   3,673.38 ns |  1.12 |    0.01 | 0.7477 |     - |     - |    2352 B |
    |             |                |               |               |             |             |               |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  29,909.45 ns | 206.3433 ns | 193.0137 ns |  29,939.07 ns |  0.90 |    0.01 | 5.2795 |     - |     - |   16640 B |
    |  SystemLinq |     FSharpList |          1000 |  33,312.42 ns | 196.7688 ns | 184.0577 ns |  33,297.52 ns |  1.00 |    0.00 | 5.3101 |     - |     - |   16760 B |
    | CisternLinq |     FSharpList |          1000 |  36,051.67 ns | 352.1144 ns | 329.3680 ns |  35,909.33 ns |  1.08 |    0.01 | 5.3101 |     - |     - |   16762 B |
    */
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
