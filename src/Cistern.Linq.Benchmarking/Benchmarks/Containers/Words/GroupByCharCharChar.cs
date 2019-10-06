using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev |           Median | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |         885.4 ns |         2.836 ns |         2.514 ns |         885.3 ns |  0.49 |    0.00 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       1,803.1 ns |         3.602 ns |         3.193 ns |       1,802.8 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       1,781.6 ns |         4.569 ns |         4.050 ns |       1,781.2 ns |  0.99 |    0.00 |    0.7420 |         - |        - |     2.27 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |      89,757.4 ns |       304.165 ns |       284.516 ns |      89,803.1 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     179,299.6 ns |     2,174.602 ns |     2,034.124 ns |     179,270.6 ns |  1.00 |    0.00 |   50.7813 |   13.6719 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     157,954.1 ns |     1,026.034 ns |       959.753 ns |     157,648.1 ns |  0.88 |    0.01 |   40.7715 |   11.7188 |        - |    137.8 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  76,949,717.6 ns |   344,237.315 ns |   287,453.748 ns |  76,959,557.1 ns |  0.81 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 |  95,291,314.8 ns | 1,788,472.488 ns | 1,913,645.525 ns |  95,939,875.0 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16542.13 KB |
    | CisternLinq |          Array |  False |     466544 |  85,665,717.1 ns |   778,772.381 ns |   728,464.169 ns |  85,388,914.3 ns |  0.90 |    0.02 | 2142.8571 | 1000.0000 | 285.7143 | 11647.71 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         774.8 ns |         1.482 ns |         1.314 ns |         774.7 ns |  0.49 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,579.8 ns |         4.094 ns |         3.419 ns |       1,578.8 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,530.4 ns |        22.735 ns |        20.154 ns |       1,524.3 ns |  0.97 |    0.01 |    0.6542 |         - |        - |     2.01 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      53,621.3 ns |       410.055 ns |       383.565 ns |      53,475.8 ns |  0.76 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      70,304.4 ns |       984.046 ns |       872.331 ns |      70,196.9 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      55,905.9 ns |       679.802 ns |       635.887 ns |      55,885.8 ns |  0.79 |    0.01 |   14.0991 |         - |        - |    43.23 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  39,439,613.3 ns |   725,197.353 ns |   678,350.054 ns |  39,579,484.6 ns |  0.70 |    0.02 | 2076.9231 |  846.1538 | 230.7692 | 11399.27 KB |
    |  SystemLinq |          Array |   True |     466544 |  56,656,658.3 ns | 1,105,793.732 ns | 1,086,037.303 ns |  56,957,688.9 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.66 KB |
    | CisternLinq |          Array |   True |     466544 |  41,345,308.2 ns |   745,266.004 ns |   697,122.282 ns |  41,126,630.8 ns |  0.73 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.51 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |         989.7 ns |        19.353 ns |        19.875 ns |         987.0 ns |  0.51 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       1,948.0 ns |        38.784 ns |        36.279 ns |       1,931.2 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       1,965.4 ns |         8.325 ns |         6.952 ns |       1,963.8 ns |  1.01 |    0.02 |    0.7477 |         - |        - |      2.3 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |      97,334.3 ns |     1,246.391 ns |     1,104.893 ns |      96,883.0 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     190,965.2 ns |     3,264.811 ns |     3,352.721 ns |     191,620.9 ns |  1.00 |    0.00 |   49.8047 |   14.4043 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     173,727.8 ns |     1,200.455 ns |     1,064.172 ns |     174,165.0 ns |  0.91 |    0.02 |   41.0156 |   10.2539 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  82,333,849.0 ns | 1,663,370.608 ns | 2,489,654.806 ns |  81,051,664.3 ns |  0.83 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 |  99,653,660.0 ns | 1,257,947.420 ns | 1,176,684.797 ns |  99,464,950.0 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.75 KB |
    | CisternLinq |           List |  False |     466544 |  83,995,677.8 ns | 1,187,055.298 ns | 1,110,372.263 ns |  83,764,983.3 ns |  0.84 |    0.01 | 2000.0000 |  833.3333 | 166.6667 | 11647.57 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         837.1 ns |         2.651 ns |         2.479 ns |         837.5 ns |  0.51 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,652.4 ns |        16.298 ns |        14.448 ns |       1,650.3 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       1,692.3 ns |        17.508 ns |        16.377 ns |       1,684.1 ns |  1.02 |    0.01 |    0.6618 |         - |        - |     2.03 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      58,125.0 ns |       793.148 ns |       741.911 ns |      57,929.8 ns |  0.79 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      73,519.3 ns |       656.283 ns |       581.778 ns |      73,269.6 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      62,259.4 ns |       705.620 ns |       660.037 ns |      62,235.1 ns |  0.85 |    0.01 |   14.0381 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  40,665,646.2 ns |   483,031.861 ns |   428,195.163 ns |  40,757,019.2 ns |  0.70 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.14 KB |
    |  SystemLinq |           List |   True |     466544 |  57,707,385.9 ns |   840,793.253 ns |   786,478.531 ns |  57,268,722.2 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  16540.6 KB |
    | CisternLinq |           List |   True |     466544 |  43,276,741.7 ns |   850,401.884 ns |   795,466.450 ns |  42,914,625.0 ns |  0.75 |    0.02 | 2000.0000 |  833.3333 | 250.0000 |  10730.6 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |         934.0 ns |         5.815 ns |         5.155 ns |         932.1 ns |  0.47 |    0.00 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       1,988.2 ns |        21.002 ns |        18.618 ns |       1,979.7 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       2,087.9 ns |        40.400 ns |        39.678 ns |       2,073.3 ns |  1.05 |    0.02 |    0.7629 |         - |        - |     2.34 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |      95,640.3 ns |       856.207 ns |       759.005 ns |      95,736.3 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     194,271.9 ns |     3,709.760 ns |     3,809.650 ns |     193,894.8 ns |  1.00 |    0.00 |   50.2930 |   14.4043 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     181,293.6 ns |     2,017.308 ns |     1,684.543 ns |     181,092.9 ns |  0.94 |    0.02 |   39.7949 |   11.4746 |        - |   137.89 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  80,494,881.0 ns | 1,587,834.805 ns | 1,485,261.661 ns |  79,890,157.1 ns |  0.81 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 |  98,976,370.0 ns | 1,930,796.160 ns | 2,065,930.260 ns |  98,922,860.0 ns |  1.00 |    0.00 | 3000.0000 | 1400.0000 | 400.0000 | 16540.55 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  84,071,992.9 ns |   951,424.829 ns |   843,413.330 ns |  83,900,500.0 ns |  0.85 |    0.02 | 2000.0000 |  833.3333 | 166.6667 | 11648.97 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         820.8 ns |         9.160 ns |         8.568 ns |         818.1 ns |  0.48 |    0.01 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,707.2 ns |        24.927 ns |        23.316 ns |       1,692.7 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       1,744.5 ns |         9.250 ns |         7.724 ns |       1,745.7 ns |  1.02 |    0.02 |    0.6771 |         - |        - |     2.08 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      53,212.8 ns |       124.992 ns |       116.917 ns |      53,204.9 ns |  0.74 |    0.00 |   13.6108 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      71,940.8 ns |       414.291 ns |       387.528 ns |      71,817.5 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      59,180.5 ns |       279.401 ns |       233.312 ns |      59,125.2 ns |  0.82 |    0.01 |   14.0991 |         - |        - |     43.3 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  40,669,831.9 ns |   536,388.759 ns |   501,738.378 ns |  40,816,871.4 ns |  0.72 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.56 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  56,861,579.3 ns |   766,113.727 ns |   716,623.256 ns |  56,941,700.0 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.59 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  42,219,409.3 ns |   461,424.011 ns |   409,040.367 ns |  42,153,003.8 ns |  0.74 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10731.28 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |         899.0 ns |         1.626 ns |         1.521 ns |         899.4 ns |  0.47 |    0.00 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       1,927.3 ns |         8.678 ns |         8.118 ns |       1,923.4 ns |  1.00 |    0.00 |    0.9384 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       1,822.4 ns |         4.318 ns |         4.039 ns |       1,823.1 ns |  0.95 |    0.00 |    0.7458 |         - |        - |     2.29 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |      89,783.3 ns |       285.703 ns |       267.247 ns |      89,765.9 ns |  0.49 |    0.00 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     184,817.1 ns |       838.934 ns |       784.739 ns |     184,757.3 ns |  1.00 |    0.00 |   49.8047 |   13.6719 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     160,282.4 ns |     1,161.694 ns |     1,086.649 ns |     160,354.8 ns |  0.87 |    0.01 |   40.0391 |   11.4746 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  78,546,372.4 ns | 1,566,222.941 ns | 1,465,045.910 ns |  77,729,600.0 ns |  0.79 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 |  99,037,495.2 ns |   543,560.859 ns |   481,852.544 ns |  98,861,908.3 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.85 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  85,330,580.0 ns |   555,189.047 ns |   519,324.179 ns |  85,233,614.3 ns |  0.86 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11647.91 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         740.4 ns |         1.856 ns |         1.736 ns |         740.7 ns |  0.47 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,585.7 ns |         6.977 ns |         6.526 ns |       1,585.6 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       1,531.0 ns |         3.631 ns |         3.397 ns |       1,532.3 ns |  0.97 |    0.00 |    0.6599 |         - |        - |     2.02 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      51,389.3 ns |       133.701 ns |       125.064 ns |      51,362.1 ns |  0.73 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      70,191.3 ns |       348.800 ns |       326.268 ns |      70,134.0 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      53,301.4 ns |        99.326 ns |        88.050 ns |      53,284.8 ns |  0.76 |    0.00 |   14.0991 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  40,344,344.8 ns |   583,833.020 ns |   546,117.769 ns |  40,222,035.7 ns |  0.71 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.62 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  56,620,591.9 ns |   562,335.143 ns |   526,008.642 ns |  56,716,788.9 ns |  1.00 |    0.00 | 3000.0000 | 1333.3333 | 333.3333 | 16540.17 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  39,901,183.1 ns |   642,182.731 ns |   600,698.125 ns |  39,839,261.5 ns |  0.70 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.89 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       1,824.8 ns |         2.438 ns |         2.281 ns |       1,824.6 ns |  0.65 |    0.00 |    0.6161 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       2,822.8 ns |        10.222 ns |         9.561 ns |       2,817.4 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       2,953.9 ns |         8.169 ns |         7.641 ns |       2,954.2 ns |  1.05 |    0.00 |    0.7591 |         - |        - |     2.33 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     173,328.4 ns |       569.148 ns |       532.381 ns |     173,194.8 ns |  0.64 |    0.00 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     271,761.5 ns |     1,616.396 ns |     1,349.764 ns |     271,747.8 ns |  1.00 |    0.00 |   50.2930 |   13.1836 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     246,960.9 ns |       730.369 ns |       647.453 ns |     247,116.2 ns |  0.91 |    0.00 |   39.0625 |   12.6953 |        - |   137.86 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 121,257,189.3 ns |   441,470.461 ns |   412,951.743 ns | 121,337,060.0 ns |  0.92 |    0.01 | 1800.0000 |  800.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 131,747,908.3 ns |   660,316.472 ns |   617,660.438 ns | 131,723,600.0 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 121,345,029.3 ns |   529,174.806 ns |   494,990.442 ns | 121,262,640.0 ns |  0.92 |    0.01 | 1800.0000 |  800.0000 |        - | 11647.87 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,732.9 ns |         2.996 ns |         2.802 ns |       1,733.0 ns |  0.69 |    0.00 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,504.5 ns |        15.241 ns |        14.257 ns |       2,502.3 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       2,677.7 ns |         6.136 ns |         5.739 ns |       2,678.1 ns |  1.07 |    0.01 |    0.6714 |         - |        - |     2.06 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     126,518.6 ns |       158.418 ns |       140.433 ns |     126,509.2 ns |  0.84 |    0.00 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     149,788.2 ns |       513.333 ns |       480.172 ns |     149,710.8 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     135,891.1 ns |       757.498 ns |       708.564 ns |     135,829.0 ns |  0.91 |    0.00 |   13.9160 |         - |        - |    43.29 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  69,125,277.5 ns |   546,389.247 ns |   511,092.841 ns |  69,096,775.0 ns |  0.80 |    0.01 | 1875.0000 |  750.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  86,558,068.9 ns | 1,025,668.579 ns |   959,411.025 ns |  86,599,916.7 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 |  16540.2 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  72,323,953.3 ns |   735,882.671 ns |   688,345.107 ns |  72,253,000.0 ns |  0.84 |    0.01 | 1875.0000 |  750.0000 | 125.0000 | 10730.66 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |         917.2 ns |         3.045 ns |         2.849 ns |         916.4 ns |  0.50 |    0.00 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       1,825.0 ns |         4.465 ns |         3.958 ns |       1,824.5 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,026.6 ns |         6.646 ns |         6.217 ns |       2,026.5 ns |  1.11 |    0.00 |    0.7439 |         - |        - |     2.29 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |      92,653.8 ns |       395.106 ns |       369.583 ns |      92,613.7 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     187,042.4 ns |     2,133.346 ns |     1,995.533 ns |     187,677.0 ns |  1.00 |    0.00 |   50.0488 |   14.6484 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     168,869.9 ns |     1,375.550 ns |     1,286.690 ns |     168,916.0 ns |  0.90 |    0.01 |   40.0391 |   12.6953 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  77,595,824.5 ns |   392,756.060 ns |   348,168.018 ns |  77,634,842.9 ns |  0.79 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 |  97,744,523.3 ns | 1,181,818.547 ns | 1,105,473.802 ns |  98,168,266.7 ns |  1.00 |    0.00 | 3000.0000 | 1333.3333 | 333.3333 | 16540.25 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  80,580,306.7 ns | 1,081,117.701 ns | 1,011,278.168 ns |  80,182,614.3 ns |  0.82 |    0.02 | 2000.0000 |  857.1429 | 142.8571 | 11647.82 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         799.9 ns |         1.436 ns |         1.344 ns |         800.6 ns |  0.48 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,662.2 ns |         6.986 ns |         6.535 ns |       1,662.5 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       1,627.3 ns |         4.498 ns |         4.208 ns |       1,626.0 ns |  0.98 |    0.00 |    0.6599 |         - |        - |     2.02 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      53,982.5 ns |       130.334 ns |       121.914 ns |      53,978.6 ns |  0.75 |    0.00 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      72,349.4 ns |       236.479 ns |       221.203 ns |      72,274.8 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      60,593.0 ns |       112.487 ns |        99.717 ns |      60,580.0 ns |  0.84 |    0.00 |   14.0991 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  39,370,584.6 ns |   431,408.034 ns |   403,539.343 ns |  39,434,738.5 ns |  0.65 |    0.01 | 2000.0000 |  846.1538 | 153.8462 | 11399.15 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  60,279,921.5 ns | 1,083,674.388 ns | 1,013,669.695 ns |  60,138,166.7 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.26 KB |
    | CisternLinq |     FSharpList |   True |     466544 |  42,297,539.0 ns |   722,451.631 ns |   675,781.704 ns |  42,400,215.4 ns |  0.70 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.89 KB |
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
