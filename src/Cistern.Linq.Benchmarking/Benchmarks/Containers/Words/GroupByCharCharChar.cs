using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |         955.8 ns |        10.781 ns |        10.084 ns |  0.50 |    0.01 |    0.6027 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       1,925.0 ns |        22.015 ns |        20.592 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       2,037.0 ns |        18.078 ns |        16.910 ns |  1.06 |    0.01 |    0.7896 |         - |        - |     2.43 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |      93,473.8 ns |       969.674 ns |       907.034 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     188,814.5 ns |     3,889.550 ns |     3,638.288 ns |  1.00 |    0.00 |   50.7813 |   14.1602 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     165,977.1 ns |     1,913.079 ns |     1,695.894 ns |  0.88 |    0.02 |   42.9688 |   10.0098 |        - |   137.95 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  79,766,006.7 ns | 1,426,266.904 ns | 1,334,130.946 ns |  0.83 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 |  95,783,470.2 ns |   695,798.493 ns |   616,807.241 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.17 KB |
    | CisternLinq |          Array |  False |     466544 |  71,171,206.7 ns |   837,270.463 ns |   783,183.310 ns |  0.74 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11647.85 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         756.7 ns |         8.137 ns |         7.213 ns |  0.46 |    0.01 |    0.5474 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,663.5 ns |        19.242 ns |        17.999 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,726.8 ns |        17.818 ns |        14.879 ns |  1.04 |    0.01 |    0.7057 |         - |        - |     2.16 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      53,472.8 ns |       622.444 ns |       582.234 ns |  0.76 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      70,299.0 ns |       673.060 ns |       629.581 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      59,158.4 ns |       594.264 ns |       555.875 ns |  0.84 |    0.01 |   14.0991 |         - |        - |    43.38 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  39,271,532.4 ns |   575,592.074 ns |   538,409.183 ns |  0.74 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.23 KB |
    |  SystemLinq |          Array |   True |     466544 |  53,421,580.0 ns |   610,892.215 ns |   571,428.957 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.22 KB |
    | CisternLinq |          Array |   True |     466544 |  42,251,518.5 ns |   400,038.410 ns |   374,196.177 ns |  0.79 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.69 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |         928.2 ns |         7.293 ns |         6.822 ns |  0.48 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       1,940.2 ns |        30.513 ns |        28.542 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       2,159.0 ns |        29.680 ns |        27.763 ns |  1.11 |    0.02 |    0.7973 |         - |        - |     2.45 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |      97,674.6 ns |     1,480.234 ns |     1,384.612 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     192,591.9 ns |     2,606.558 ns |     2,310.646 ns |  1.00 |    0.00 |   50.0488 |   15.8691 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     176,880.2 ns |     2,312.466 ns |     2,163.082 ns |  0.92 |    0.01 |   42.9688 |   10.9863 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  82,767,171.1 ns | 1,075,873.104 ns | 1,006,372.369 ns |  0.85 |    0.02 | 1833.3333 |  833.3333 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 |  97,514,538.9 ns | 1,257,188.431 ns | 1,175,974.839 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.02 KB |
    | CisternLinq |           List |  False |     466544 |  77,437,317.1 ns |   606,500.751 ns |   567,321.179 ns |  0.79 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11648.95 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         872.4 ns |        12.002 ns |        11.227 ns |  0.53 |    0.01 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,632.7 ns |        23.043 ns |        21.555 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       1,853.7 ns |        23.092 ns |        21.600 ns |  1.14 |    0.02 |    0.7133 |         - |        - |     2.19 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      57,078.6 ns |       734.177 ns |       686.749 ns |  0.78 |    0.02 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      73,151.3 ns |     1,172.895 ns |     1,097.127 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      67,049.3 ns |       878.439 ns |       821.692 ns |  0.92 |    0.02 |   14.1602 |         - |        - |    43.41 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  39,509,863.1 ns |   535,492.953 ns |   500,900.441 ns |  0.70 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.53 KB |
    |  SystemLinq |           List |   True |     466544 |  56,254,183.3 ns |   649,901.692 ns |   607,918.446 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16541.35 KB |
    | CisternLinq |           List |   True |     466544 |  44,540,692.8 ns |   511,277.622 ns |   478,249.405 ns |  0.79 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10730.73 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |         898.2 ns |        11.444 ns |        10.705 ns |  0.46 |    0.00 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       1,963.5 ns |        27.846 ns |        26.047 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       2,221.0 ns |        21.507 ns |        20.118 ns |  1.13 |    0.02 |    0.8125 |         - |        - |      2.5 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |      96,617.7 ns |     1,901.519 ns |     3,016.011 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     194,128.1 ns |     2,372.248 ns |     2,102.936 ns |  1.00 |    0.00 |   50.0488 |   13.1836 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     173,115.1 ns |     2,547.178 ns |     2,382.632 ns |  0.89 |    0.01 |   42.7246 |   10.2539 |        - |   138.04 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  81,213,781.2 ns | 1,538,465.981 ns | 1,889,374.387 ns |  0.81 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 |  99,784,857.8 ns |   453,755.475 ns |   424,443.152 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.22 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  75,934,527.6 ns |   579,514.951 ns |   542,078.645 ns |  0.76 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 |  11649.3 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         847.0 ns |         9.242 ns |         8.645 ns |  0.51 |    0.01 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,669.9 ns |        21.256 ns |        19.883 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       1,954.7 ns |        38.332 ns |        35.856 ns |  1.17 |    0.02 |    0.7286 |         - |        - |     2.23 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      55,141.1 ns |       829.422 ns |       775.842 ns |  0.73 |    0.01 |   13.6108 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      75,166.9 ns |       876.184 ns |       819.583 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      66,543.9 ns |       975.701 ns |       912.671 ns |  0.89 |    0.02 |   14.1602 |         - |        - |    43.46 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  39,781,917.1 ns |   480,532.281 ns |   449,490.193 ns |  0.71 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.67 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  56,176,784.7 ns |   672,593.799 ns |   629,144.657 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.81 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  44,136,670.6 ns |   584,455.479 ns |   546,700.017 ns |  0.79 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10731.55 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |         936.4 ns |        13.594 ns |        12.716 ns |  0.47 |    0.01 |    0.6027 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       2,007.1 ns |        18.222 ns |        17.045 ns |  1.00 |    0.00 |    0.9384 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       2,223.7 ns |        30.209 ns |        28.257 ns |  1.11 |    0.02 |    0.7973 |         - |        - |     2.45 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |      94,093.2 ns |       909.559 ns |       850.802 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     193,804.8 ns |     2,700.946 ns |     2,526.466 ns |  1.00 |    0.00 |   50.7813 |   14.1602 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     166,290.7 ns |     1,600.488 ns |     1,497.097 ns |  0.86 |    0.01 |   42.9688 |   10.9863 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  81,853,937.4 ns | 1,137,372.675 ns |   949,757.696 ns |  0.83 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 |  98,349,386.7 ns | 1,264,470.569 ns | 1,182,786.555 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.39 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  70,646,089.2 ns |   631,920.814 ns |   591,099.121 ns |  0.72 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.16 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         886.5 ns |        13.114 ns |        12.267 ns |  0.53 |    0.01 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,686.8 ns |        23.910 ns |        22.365 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       1,842.3 ns |        25.133 ns |        23.509 ns |  1.09 |    0.02 |    0.7114 |         - |        - |     2.18 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      53,450.2 ns |       724.179 ns |       677.397 ns |  0.72 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      74,525.0 ns |       714.275 ns |       668.133 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      59,198.3 ns |       915.238 ns |       856.114 ns |  0.79 |    0.01 |   14.1602 |         - |        - |     43.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  39,365,873.8 ns |   466,354.030 ns |   436,227.849 ns |  0.71 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.14 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  55,452,324.7 ns |   633,496.404 ns |   592,572.930 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.51 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  42,384,444.1 ns |   379,051.141 ns |   354,564.673 ns |  0.76 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10731.39 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       2,012.5 ns |        16.310 ns |        15.257 ns |  0.66 |    0.01 |    0.6142 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       3,068.6 ns |        30.095 ns |        28.151 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       3,424.4 ns |        43.317 ns |        40.518 ns |  1.12 |    0.02 |    0.8087 |         - |        - |     2.48 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     181,472.2 ns |     1,541.389 ns |     1,441.817 ns |  0.65 |    0.01 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     281,147.0 ns |     3,929.969 ns |     3,676.096 ns |  1.00 |    0.00 |   49.3164 |   14.1602 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     261,285.6 ns |     2,483.886 ns |     2,323.428 ns |  0.93 |    0.01 |   43.4570 |   12.2070 |        - |   138.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 129,426,788.0 ns | 1,007,479.390 ns |   942,396.846 ns |  0.91 |    0.01 | 1800.0000 |  800.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 142,723,576.7 ns | 2,130,546.338 ns | 1,992,914.366 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 120,335,225.3 ns | 1,829,554.565 ns | 1,711,366.475 ns |  0.84 |    0.02 | 2000.0000 | 1000.0000 |        - | 11648.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,707.5 ns |        16.856 ns |        15.767 ns |  0.65 |    0.01 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,636.4 ns |        11.328 ns |         9.460 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       3,093.1 ns |        28.563 ns |        26.718 ns |  1.17 |    0.01 |    0.7210 |         - |        - |     2.22 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     133,589.4 ns |     1,799.882 ns |     1,683.611 ns |  0.86 |    0.01 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     155,794.1 ns |     1,879.072 ns |     1,757.685 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     150,078.5 ns |     1,913.657 ns |     1,790.036 ns |  0.96 |    0.02 |   14.1602 |         - |        - |    43.44 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  74,342,927.7 ns | 1,462,507.267 ns | 1,501,887.286 ns |  0.83 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  89,217,664.4 ns | 1,014,007.799 ns |   948,503.524 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 | 16540.87 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  80,225,688.1 ns | 1,532,295.656 ns | 1,639,539.185 ns |  0.90 |    0.02 | 1857.1429 |  714.2857 | 142.8571 |  10730.9 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |         976.6 ns |         9.792 ns |         9.159 ns |  0.51 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       1,918.5 ns |        24.580 ns |        22.992 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,327.0 ns |        26.348 ns |        24.646 ns |  1.21 |    0.02 |    0.7973 |         - |        - |     2.45 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |      96,547.3 ns |       886.300 ns |       829.046 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     192,053.5 ns |     2,750.530 ns |     2,572.847 ns |  1.00 |    0.00 |   50.0488 |   14.6484 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     177,126.6 ns |     2,518.775 ns |     2,356.064 ns |  0.92 |    0.02 |   42.7246 |   11.9629 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  82,924,743.3 ns | 1,316,425.222 ns | 1,231,384.969 ns |  0.83 |    0.02 | 1833.3333 |  833.3333 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 |  99,458,254.7 ns | 1,316,877.507 ns | 1,231,808.037 ns |  1.00 |    0.00 | 2800.0000 | 1400.0000 | 200.0000 | 16540.46 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  71,604,237.1 ns | 1,139,831.285 ns | 1,066,198.891 ns |  0.72 |    0.01 | 2000.0000 |  857.1429 | 142.8571 | 11648.19 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         773.8 ns |         6.812 ns |         6.372 ns |  0.46 |    0.01 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,670.2 ns |        21.075 ns |        19.713 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       1,986.3 ns |        28.439 ns |        26.602 ns |  1.19 |    0.02 |    0.7095 |         - |        - |     2.18 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      55,749.4 ns |       795.910 ns |       744.495 ns |  0.74 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      75,086.7 ns |       890.585 ns |       833.054 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      69,360.6 ns |       982.492 ns |       919.023 ns |  0.92 |    0.02 |   14.1602 |         - |        - |     43.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  39,578,748.2 ns |   482,856.698 ns |   451,664.455 ns |  0.70 |    0.01 | 2000.0000 |  846.1538 | 153.8462 | 11398.87 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  56,306,834.0 ns |   574,615.694 ns |   537,495.877 ns |  1.00 |    0.00 | 3000.0000 | 1300.0000 | 300.0000 | 16540.34 KB |
    | CisternLinq |     FSharpList |   True |     466544 |  44,241,231.7 ns |   551,067.139 ns |   515,468.544 ns |  0.79 |    0.01 | 1916.6667 |  750.0000 | 166.6667 |  10730.6 KB |
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
