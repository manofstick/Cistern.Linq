using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |         966.8 ns |         7.143 ns |         6.681 ns |  0.50 |    0.01 |    0.6027 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       1,935.4 ns |        32.606 ns |        30.499 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       1,955.9 ns |        33.885 ns |        31.696 ns |  1.01 |    0.02 |    0.7515 |         - |        - |      2.3 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |      93,266.7 ns |       965.663 ns |       903.281 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     185,360.8 ns |     2,502.284 ns |     2,218.210 ns |  1.00 |    0.00 |   50.2930 |   14.4043 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     164,392.5 ns |     1,883.487 ns |     1,761.815 ns |  0.89 |    0.01 |   42.7246 |   10.2539 |        - |   137.83 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  80,673,217.9 ns | 1,604,491.516 ns | 1,847,734.815 ns |  0.85 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 |  94,767,566.7 ns | 1,114,180.806 ns | 1,042,205.418 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.37 KB |
    | CisternLinq |          Array |  False |     466544 |  68,115,349.2 ns |   942,365.751 ns |   881,489.508 ns |  0.72 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11647.58 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         786.6 ns |        10.695 ns |        10.004 ns |  0.50 |    0.01 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,579.9 ns |        18.617 ns |        17.415 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,702.7 ns |        23.218 ns |        21.718 ns |  1.08 |    0.02 |    0.6657 |         - |        - |     2.04 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      54,082.2 ns |       597.085 ns |       558.514 ns |  0.78 |    0.02 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      68,998.1 ns |     1,187.341 ns |     1,110.639 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      57,000.7 ns |       786.751 ns |       697.434 ns |  0.83 |    0.01 |   14.0991 |         - |        - |    43.26 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  38,024,837.9 ns |   500,731.276 ns |   443,885.234 ns |  0.70 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11398.96 KB |
    |  SystemLinq |          Array |   True |     466544 |  54,336,098.0 ns |   358,910.820 ns |   335,725.404 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.94 KB |
    | CisternLinq |          Array |   True |     466544 |  41,617,582.1 ns |   505,814.497 ns |   473,139.194 ns |  0.77 |    0.01 | 2000.0000 |  846.1538 | 230.7692 |  10730.7 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |         908.7 ns |         9.844 ns |         9.208 ns |  0.46 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       1,987.7 ns |        24.087 ns |        22.531 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       2,115.8 ns |        19.673 ns |        18.402 ns |  1.06 |    0.02 |    0.7591 |         - |        - |     2.33 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |      98,176.8 ns |     1,176.283 ns |     1,100.296 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     193,884.7 ns |     2,098.827 ns |     1,963.244 ns |  1.00 |    0.00 |   49.5605 |   14.8926 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     173,168.6 ns |     2,842.873 ns |     2,659.226 ns |  0.89 |    0.01 |   42.4805 |   10.9863 |        - |   137.85 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  82,984,333.3 ns | 1,485,444.365 ns | 1,389,485.581 ns |  0.84 |    0.02 | 1833.3333 |  833.3333 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 |  98,270,326.2 ns |   880,955.958 ns |   780,944.510 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.68 KB |
    | CisternLinq |           List |  False |     466544 |  73,276,674.3 ns |   366,756.729 ns |   343,064.472 ns |  0.75 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11648.48 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         883.6 ns |         8.268 ns |         7.329 ns |  0.53 |    0.01 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,659.8 ns |        26.588 ns |        22.202 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       1,754.3 ns |        25.464 ns |        23.819 ns |  1.06 |    0.02 |    0.6733 |         - |        - |     2.06 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      57,873.1 ns |       639.816 ns |       598.484 ns |  0.77 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      74,896.9 ns |       655.368 ns |       613.032 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      65,040.1 ns |       871.324 ns |       815.037 ns |  0.87 |    0.01 |   14.0381 |         - |        - |    43.28 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  39,612,105.6 ns |   639,195.488 ns |   597,903.856 ns |  0.71 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.21 KB |
    |  SystemLinq |           List |   True |     466544 |  55,857,913.3 ns |   626,789.791 ns |   586,299.559 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.32 KB |
    | CisternLinq |           List |   True |     466544 |  43,731,641.1 ns |   427,607.665 ns |   399,984.476 ns |  0.78 |    0.01 | 2000.0000 |  833.3333 | 250.0000 |  10730.9 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |         913.5 ns |        11.123 ns |        10.405 ns |  0.46 |    0.01 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       1,997.2 ns |        24.858 ns |        23.253 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       2,136.4 ns |        23.112 ns |        21.619 ns |  1.07 |    0.02 |    0.7744 |         - |        - |     2.38 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |      96,213.0 ns |       979.215 ns |       915.959 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     194,691.7 ns |     2,212.735 ns |     2,069.793 ns |  1.00 |    0.00 |   49.8047 |   14.4043 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     173,833.0 ns |     2,574.844 ns |     2,408.511 ns |  0.89 |    0.02 |   42.7246 |   10.9863 |        - |   137.92 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  82,136,672.4 ns | 1,639,007.886 ns | 1,452,937.800 ns |  0.81 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 | 101,254,728.9 ns |   973,280.974 ns |   910,407.627 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.53 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  73,074,144.8 ns |   800,502.397 ns |   748,790.439 ns |  0.72 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11649.31 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         823.9 ns |         7.346 ns |         6.872 ns |  0.49 |    0.01 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,675.4 ns |        18.873 ns |        17.654 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       1,828.4 ns |        23.431 ns |        21.918 ns |  1.09 |    0.02 |    0.6886 |         - |        - |     2.11 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      54,980.2 ns |       779.358 ns |       729.012 ns |  0.74 |    0.01 |   13.6108 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      74,331.5 ns |       791.468 ns |       740.339 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      64,185.7 ns |       941.349 ns |       880.539 ns |  0.86 |    0.01 |   14.0381 |         - |        - |    43.33 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  38,863,501.5 ns |   637,548.043 ns |   596,362.835 ns |  0.69 |    0.02 | 2076.9231 |  846.1538 | 230.7692 | 11398.94 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  56,043,023.3 ns |   672,253.310 ns |   628,826.164 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16541.11 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  42,989,815.0 ns |   626,481.172 ns |   586,010.877 ns |  0.77 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10730.91 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |         918.1 ns |        11.814 ns |        11.050 ns |  0.47 |    0.01 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       1,958.9 ns |        16.959 ns |        15.864 ns |  1.00 |    0.00 |    0.9384 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       2,172.4 ns |        23.859 ns |        22.317 ns |  1.11 |    0.02 |    0.7553 |         - |        - |     2.32 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |      93,386.7 ns |       906.896 ns |       848.311 ns |  0.48 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     193,148.5 ns |     2,467.859 ns |     2,308.437 ns |  1.00 |    0.00 |   50.7813 |   14.1602 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     166,977.5 ns |     2,666.768 ns |     2,494.497 ns |  0.86 |    0.02 |   41.7480 |   10.2539 |        - |   137.86 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  81,608,770.4 ns | 1,509,340.691 ns | 1,337,991.209 ns |  0.82 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 |  99,516,396.7 ns |   958,187.115 ns |   896,288.822 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.39 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  68,423,235.8 ns |   624,857.082 ns |   584,491.702 ns |  0.69 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.25 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         796.8 ns |        10.667 ns |         9.978 ns |  0.49 |    0.01 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,633.4 ns |        16.930 ns |        15.836 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       1,722.5 ns |        21.694 ns |        20.292 ns |  1.05 |    0.02 |    0.6695 |         - |        - |     2.05 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      52,985.4 ns |       467.104 ns |       436.930 ns |  0.72 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      73,807.3 ns |       924.676 ns |       864.943 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      57,572.3 ns |       291.946 ns |       273.087 ns |  0.78 |    0.01 |   14.0991 |         - |        - |    43.28 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  39,339,839.5 ns |   432,957.954 ns |   404,989.139 ns |  0.70 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.46 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  55,852,534.7 ns |   593,201.241 ns |   554,880.809 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16540.65 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  41,159,705.1 ns |   491,839.623 ns |   460,067.089 ns |  0.74 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.87 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       1,940.2 ns |        27.243 ns |        25.483 ns |  0.66 |    0.01 |    0.6142 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       2,934.6 ns |        29.501 ns |        27.595 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       3,220.4 ns |        37.430 ns |        35.012 ns |  1.10 |    0.01 |    0.7668 |         - |        - |     2.36 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     182,549.8 ns |     1,945.797 ns |     1,820.100 ns |  0.63 |    0.01 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     289,500.4 ns |     1,419.440 ns |     1,327.745 ns |  1.00 |    0.00 |   49.3164 |   15.1367 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     251,774.7 ns |     4,828.560 ns |     6,106.582 ns |  0.86 |    0.02 |   41.5039 |   10.2539 |        - |   137.89 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 127,870,991.1 ns |   757,748.872 ns |   671,724.638 ns |  0.92 |    0.01 | 1750.0000 |  750.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 138,943,711.7 ns | 1,642,590.778 ns | 1,536,480.433 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 116,383,685.3 ns | 1,265,228.039 ns | 1,183,495.093 ns |  0.84 |    0.02 | 1800.0000 |  800.0000 |        - |  11647.9 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,828.1 ns |        18.592 ns |        17.391 ns |  0.67 |    0.01 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,726.2 ns |        33.656 ns |        31.482 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       2,926.8 ns |        36.230 ns |        33.889 ns |  1.07 |    0.02 |    0.6828 |         - |        - |     2.09 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     135,011.4 ns |     1,159.045 ns |     1,084.171 ns |  0.88 |    0.02 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     153,713.6 ns |     2,264.467 ns |     2,118.184 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     145,493.1 ns |     1,986.576 ns |     1,858.245 ns |  0.95 |    0.02 |   13.9160 |         - |        - |    43.32 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  75,609,724.8 ns | 1,510,506.329 ns | 1,412,928.557 ns |  0.85 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  88,485,027.4 ns |   252,601.790 ns |   223,924.907 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 | 16540.51 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  76,012,640.2 ns | 1,454,557.833 ns | 1,428,570.284 ns |  0.86 |    0.02 | 1857.1429 |  714.2857 | 142.8571 | 10730.62 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |         985.6 ns |        12.055 ns |        11.276 ns |  0.49 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       2,022.2 ns |        24.550 ns |        22.964 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,295.2 ns |        28.276 ns |        26.449 ns |  1.14 |    0.02 |    0.7553 |         - |        - |     2.32 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |      95,170.4 ns |       887.833 ns |       830.479 ns |  0.48 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     196,863.7 ns |     3,088.213 ns |     2,888.716 ns |  1.00 |    0.00 |   49.0723 |   14.8926 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     177,496.6 ns |     2,546.350 ns |     2,381.857 ns |  0.90 |    0.02 |   41.9922 |   11.7188 |        - |   137.86 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  82,252,521.9 ns |   806,183.564 ns |   754,104.605 ns |  0.82 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 |  99,895,210.7 ns |   952,296.374 ns |   890,778.619 ns |  1.00 |    0.00 | 2800.0000 | 1400.0000 | 200.0000 |  16540.9 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  72,422,449.2 ns |   972,440.592 ns |   909,621.533 ns |  0.73 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.94 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         847.1 ns |        11.948 ns |        11.176 ns |  0.50 |    0.01 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,701.5 ns |        20.809 ns |        19.465 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       1,942.5 ns |        29.596 ns |        27.684 ns |  1.14 |    0.02 |    0.6676 |         - |        - |     2.05 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      55,597.1 ns |       698.211 ns |       653.107 ns |  0.74 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      75,651.5 ns |     1,003.031 ns |       938.236 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      66,897.2 ns |       882.261 ns |       825.267 ns |  0.88 |    0.01 |   14.0381 |         - |        - |    43.28 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  40,000,825.8 ns |   599,045.441 ns |   531,038.180 ns |  0.73 |    0.01 | 2000.0000 |  846.1538 | 153.8462 | 11399.09 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  54,741,386.7 ns | 1,011,854.226 ns |   946,489.071 ns |  1.00 |    0.00 | 3000.0000 | 1222.2222 | 333.3333 | 16540.97 KB |
| CisternLinq |     FSharpList |   True |     466544 |  43,431,893.9 ns |   550,618.730 ns |   515,049.102 ns |  0.79 |    0.01 | 1916.6667 |  750.0000 | 166.6667 |    10731 KB |    */
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
