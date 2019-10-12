using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |         836.4 ns |         2.834 ns |         2.213 ns |  0.47 |    0.00 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       1,768.8 ns |         4.788 ns |         3.998 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       2,273.6 ns |         6.933 ns |         6.146 ns |  1.29 |    0.00 |    0.9460 |         - |        - |      2.9 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |      89,734.6 ns |       243.343 ns |       227.623 ns |  0.51 |    0.00 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     175,936.0 ns |     1,304.395 ns |     1,089.229 ns |  1.00 |    0.00 |   49.8047 |   14.6484 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     195,682.8 ns |       926.759 ns |       866.891 ns |  1.11 |    0.01 |   54.1992 |   18.0664 |        - |    167.2 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  75,956,439.3 ns |   335,374.613 ns |   261,838.438 ns |  0.79 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 |  95,816,442.9 ns |   994,761.179 ns |   881,829.875 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.14 KB |
    | CisternLinq |          Array |  False |     466544 |  72,687,794.3 ns | 1,112,310.444 ns | 1,040,455.879 ns |  0.76 |    0.01 | 2428.5714 | 1142.8571 | 428.5714 |  12133.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         770.2 ns |         2.816 ns |         2.351 ns |  0.49 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,571.6 ns |         5.780 ns |         5.407 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,951.4 ns |        12.852 ns |        11.393 ns |  1.24 |    0.01 |    0.8278 |         - |        - |     2.54 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      51,505.0 ns |       249.964 ns |       233.816 ns |  0.78 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      65,905.0 ns |        87.137 ns |        72.764 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      63,991.5 ns |       466.266 ns |       364.030 ns |  0.97 |    0.01 |   15.6250 |         - |        - |    48.13 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  38,369,793.3 ns |   219,472.299 ns |   205,294.525 ns |  0.68 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.03 KB |
    |  SystemLinq |          Array |   True |     466544 |  56,541,969.3 ns | 1,040,205.454 ns |   922,115.041 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.62 KB |
    | CisternLinq |          Array |   True |     466544 |  45,966,633.9 ns |   612,700.917 ns |   573,120.818 ns |  0.81 |    0.02 | 2090.9091 |  909.0909 | 272.7273 | 11215.31 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |         950.4 ns |         6.212 ns |         5.811 ns |  0.52 |    0.00 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       1,835.5 ns |         4.224 ns |         3.951 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       2,386.6 ns |         4.883 ns |         4.329 ns |  1.30 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |      92,403.5 ns |       449.931 ns |       420.866 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     185,188.3 ns |     2,176.391 ns |     2,035.798 ns |  1.00 |    0.00 |   48.8281 |   15.6250 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     196,723.0 ns |       730.134 ns |       682.968 ns |  1.06 |    0.01 |   52.7344 |   17.0898 |        - |   167.22 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  78,112,644.9 ns |   399,888.069 ns |   354,490.358 ns |  0.80 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 |  97,685,754.4 ns | 1,157,409.355 ns | 1,082,641.429 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16542.23 KB |
    | CisternLinq |           List |  False |     466544 |  78,554,705.7 ns | 1,302,731.921 ns | 1,218,576.247 ns |  0.80 |    0.01 | 2428.5714 | 1142.8571 | 428.5714 | 12132.85 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         764.7 ns |         2.385 ns |         2.231 ns |  0.50 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,523.9 ns |         2.446 ns |         2.168 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       2,033.8 ns |         7.184 ns |         6.720 ns |  1.33 |    0.00 |    0.8354 |         - |        - |     2.56 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      55,316.5 ns |       108.950 ns |        90.979 ns |  0.79 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      70,135.8 ns |       146.956 ns |       137.463 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      71,329.9 ns |       267.345 ns |       250.075 ns |  1.02 |    0.00 |   15.6250 |         - |        - |    48.16 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  39,579,350.3 ns |   310,901.963 ns |   290,817.889 ns |  0.69 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.03 KB |
    |  SystemLinq |           List |   True |     466544 |  57,042,334.8 ns |   228,031.683 ns |   213,300.977 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  16540.7 KB |
    | CisternLinq |           List |   True |     466544 |  48,613,462.4 ns |   863,697.155 ns |   807,902.855 ns |  0.85 |    0.01 | 2090.9091 |  909.0909 | 272.7273 | 11215.51 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |         904.3 ns |         4.361 ns |         4.079 ns |  0.46 |    0.00 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       1,946.5 ns |         4.627 ns |         4.102 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       2,459.3 ns |         9.657 ns |         9.033 ns |  1.26 |    0.00 |    0.9689 |         - |        - |     2.97 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |      90,685.4 ns |       247.232 ns |       231.261 ns |  0.48 |    0.00 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     187,427.5 ns |       702.187 ns |       586.358 ns |  1.00 |    0.00 |   51.0254 |   14.1602 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     210,793.5 ns |       686.004 ns |       641.688 ns |  1.12 |    0.00 |   45.1660 |   14.6484 |        - |   167.29 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  76,589,176.9 ns |   654,647.450 ns |   546,660.270 ns |  0.78 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 |  98,779,632.2 ns |   717,698.965 ns |   671,336.058 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.37 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  77,874,475.4 ns | 1,488,691.723 ns | 1,592,883.465 ns |  0.79 |    0.02 | 2428.5714 | 1142.8571 | 428.5714 | 12133.43 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         797.8 ns |         2.294 ns |         2.034 ns |  0.49 |    0.00 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,633.9 ns |         3.374 ns |         3.156 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       2,076.0 ns |         7.560 ns |         7.071 ns |  1.27 |    0.00 |    0.8507 |         - |        - |     2.61 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      52,940.4 ns |        96.867 ns |        90.609 ns |  0.74 |    0.00 |   13.6108 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      71,658.3 ns |       305.618 ns |       285.876 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      71,655.9 ns |       191.096 ns |       178.752 ns |  1.00 |    0.00 |   15.6250 |         - |        - |    48.21 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  38,782,909.7 ns |   538,040.416 ns |   503,283.339 ns |  0.68 |    0.01 | 2076.9231 |  846.1538 | 230.7692 |  11399.3 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  56,733,866.7 ns |   604,322.402 ns |   565,283.550 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.68 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  48,699,244.8 ns |   919,156.421 ns |   859,779.487 ns |  0.86 |    0.02 | 2090.9091 |  909.0909 | 272.7273 | 11215.89 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |         881.4 ns |         3.034 ns |         2.838 ns |  0.48 |    0.00 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       1,839.8 ns |        10.084 ns |         8.939 ns |  1.00 |    0.00 |    0.9403 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       2,479.0 ns |         8.058 ns |         7.537 ns |  1.35 |    0.01 |    0.9499 |         - |        - |     2.91 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |      90,588.8 ns |       247.948 ns |       219.800 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     182,381.8 ns |     2,532.624 ns |     2,369.018 ns |  1.00 |    0.00 |   50.2930 |   13.4277 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     192,579.6 ns |       472.217 ns |       418.608 ns |  1.05 |    0.01 |   54.4434 |   18.0664 |        - |   167.23 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  76,771,901.2 ns |   266,812.031 ns |   208,309.284 ns |  0.78 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 |  99,009,377.8 ns |   804,521.527 ns |   752,549.936 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.48 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  72,134,883.8 ns | 1,242,350.178 ns | 1,162,095.127 ns |  0.73 |    0.01 | 2428.5714 | 1142.8571 | 428.5714 | 12133.31 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         751.1 ns |         1.848 ns |         1.638 ns |  0.48 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,572.2 ns |         3.124 ns |         2.609 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       2,036.7 ns |         7.543 ns |         7.056 ns |  1.30 |    0.01 |    0.8316 |         - |        - |     2.55 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      51,337.9 ns |       114.159 ns |       106.784 ns |  0.73 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      70,367.1 ns |       407.028 ns |       380.734 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      63,766.5 ns |       201.172 ns |       178.334 ns |  0.91 |    0.01 |   15.6250 |         - |        - |    48.15 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  38,491,590.1 ns |   280,396.396 ns |   248,564.101 ns |  0.68 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11398.96 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  56,711,054.8 ns |   535,318.683 ns |   500,737.428 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.57 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  45,769,185.5 ns |   862,356.877 ns |   806,649.158 ns |  0.81 |    0.01 | 2090.9091 |  909.0909 | 272.7273 | 11215.64 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       1,733.5 ns |         5.107 ns |         4.777 ns |  0.60 |    0.00 |    0.6161 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       2,873.0 ns |         4.798 ns |         4.007 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       3,609.7 ns |        11.803 ns |        11.040 ns |  1.26 |    0.00 |    0.9613 |         - |        - |     2.95 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     173,719.3 ns |       453.234 ns |       423.955 ns |  0.64 |    0.00 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     269,380.6 ns |     2,181.085 ns |     2,040.188 ns |  1.00 |    0.00 |   50.7813 |   14.1602 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     289,968.0 ns |     3,836.375 ns |     3,588.548 ns |  1.08 |    0.02 |   48.8281 |   16.1133 |        - |   167.26 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 120,653,705.3 ns |   830,397.494 ns |   776,754.331 ns |  0.91 |    0.01 | 1800.0000 |  800.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 131,990,531.7 ns |   768,782.314 ns |   719,119.454 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 118,538,816.0 ns | 1,051,407.033 ns |   983,486.791 ns |  0.90 |    0.01 | 2000.0000 | 1000.0000 |        - | 12133.08 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,656.8 ns |         5.609 ns |         5.247 ns |  0.66 |    0.00 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,514.0 ns |         9.047 ns |         8.463 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       3,157.2 ns |         9.803 ns |         9.170 ns |  1.26 |    0.01 |    0.8469 |         - |        - |     2.59 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     125,781.0 ns |       271.439 ns |       253.904 ns |  0.85 |    0.00 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     147,244.5 ns |       451.533 ns |       422.364 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     149,561.2 ns |       526.580 ns |       439.718 ns |  1.02 |    0.00 |   15.6250 |         - |        - |    48.19 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  68,640,828.3 ns |   541,167.360 ns |   506,208.284 ns |  0.80 |    0.01 | 1875.0000 |  750.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  86,239,076.2 ns |   821,037.284 ns |   727,828.167 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 | 16540.25 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  84,095,510.5 ns |   890,690.807 ns |   833,152.734 ns |  0.97 |    0.01 | 2000.0000 |  714.2857 | 142.8571 | 11216.35 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |         931.5 ns |         2.440 ns |         2.163 ns |  0.50 |    0.00 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       1,869.5 ns |         3.786 ns |         3.541 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,479.3 ns |         7.806 ns |         6.518 ns |  1.33 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |      91,901.5 ns |       277.545 ns |       259.616 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     187,824.7 ns |     2,255.313 ns |     2,109.621 ns |  1.00 |    0.00 |   49.3164 |   13.1836 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     203,519.9 ns |     1,085.384 ns |       962.165 ns |  1.08 |    0.01 |   53.4668 |   17.8223 |        - |   167.23 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  77,675,592.9 ns |   299,796.573 ns |   234,061.445 ns |  0.80 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 |  96,570,126.7 ns |   791,853.561 ns |   740,700.312 ns |  1.00 |    0.00 | 2833.3333 | 1333.3333 | 166.6667 | 16540.51 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  79,870,490.5 ns |   856,963.450 ns |   801,604.143 ns |  0.83 |    0.01 | 2285.7143 | 1142.8571 | 285.7143 | 12133.53 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         758.8 ns |         1.642 ns |         1.536 ns |  0.47 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,628.0 ns |         6.684 ns |         6.253 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       2,164.7 ns |        26.285 ns |        21.949 ns |  1.33 |    0.01 |    0.8316 |         - |        - |     2.55 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      54,271.8 ns |        80.519 ns |        75.318 ns |  0.76 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      71,775.2 ns |       212.109 ns |       198.407 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      74,446.2 ns |       162.338 ns |       126.743 ns |  1.04 |    0.00 |   15.6250 |         - |        - |    48.15 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  39,176,335.2 ns |   401,835.314 ns |   356,216.540 ns |  0.66 |    0.01 | 2000.0000 |  846.1538 | 153.8462 | 11399.35 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  59,696,570.4 ns |   446,582.549 ns |   348,662.279 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.56 KB |
    | CisternLinq |     FSharpList |   True |     466544 |  48,142,313.9 ns |   826,158.272 ns |   772,788.960 ns |  0.81 |    0.01 | 2000.0000 |  818.1818 | 181.8182 | 11215.73 KB |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_GroupByCharCharChar : WordsBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var answer = new DesiredShape();

            foreach (var n in Words)
            {
                if (n.Length >= 3)
                {
                    var key = (n[0], n[1], n[2]);
                    if (!answer.TryGetValue(key, out var words))
                    {
                        words = new List<string>();
                        answer[key] = words;
                    }
                    words.Add(n);
                }
            }

            return answer;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return
                System.Linq.Enumerable.ToDictionary(
                    System.Linq.Enumerable.GroupBy(
                        System.Linq.Enumerable.Where(Words, w => w.Length >= 3),
                            w => (w[0], w[1], w[2])),
                                ws => ws.Key,
                                ws => System.Linq.Enumerable.ToList(ws));
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Words
                .Where(w => w.Length >= 3)
                .GroupBy(w => (w[0], w[1], w[2]))
                .ToDictionary(ws => ws.Key, ws => ws.ToList());
        }
    }
}
