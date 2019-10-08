using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |         894.8 ns |        13.993 ns |        13.089 ns |  0.46 |    0.01 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       1,930.1 ns |        24.974 ns |        23.360 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       1,921.2 ns |        27.985 ns |        26.177 ns |  1.00 |    0.02 |    0.7401 |         - |        - |     2.27 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |      95,083.8 ns |       567.392 ns |       530.739 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     187,867.4 ns |     3,032.525 ns |     2,836.626 ns |  1.00 |    0.00 |   51.5137 |   15.8691 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     164,425.0 ns |     2,804.359 ns |     2,623.199 ns |  0.88 |    0.02 |   40.0391 |   12.4512 |        - |    137.8 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  80,304,702.0 ns | 1,081,240.767 ns |   958,491.777 ns |  0.86 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 |  93,575,331.1 ns |   898,324.570 ns |   840,293.361 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16541.11 KB |
    | CisternLinq |          Array |  False |     466544 |  65,798,013.3 ns |   749,369.055 ns |   700,960.279 ns |  0.70 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.34 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         762.7 ns |        10.171 ns |         9.514 ns |  0.49 |    0.01 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,547.7 ns |         8.005 ns |         7.488 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,585.7 ns |        16.297 ns |        15.244 ns |  1.02 |    0.01 |    0.6542 |         - |        - |     2.01 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      53,912.2 ns |       517.866 ns |       459.074 ns |  0.77 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      69,695.5 ns |       729.773 ns |       682.630 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      54,841.8 ns |       464.841 ns |       434.812 ns |  0.79 |    0.01 |   14.0991 |         - |        - |    43.23 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  39,110,194.4 ns |   301,301.516 ns |   267,095.946 ns |  0.72 |    0.01 | 2142.8571 |  928.5714 | 285.7143 | 11399.64 KB |
    |  SystemLinq |          Array |   True |     466544 |  54,180,480.0 ns |   599,449.343 ns |   560,725.287 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 |  16540.3 KB |
    | CisternLinq |          Array |   True |     466544 |  40,423,776.4 ns |   609,652.872 ns |   570,269.675 ns |  0.75 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10730.15 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |         951.5 ns |        12.906 ns |        12.072 ns |  0.48 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       1,977.5 ns |        14.879 ns |        13.917 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       2,057.1 ns |        29.655 ns |        27.739 ns |  1.04 |    0.02 |    0.7477 |         - |        - |      2.3 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |      97,064.6 ns |     1,027.782 ns |       961.388 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     191,156.3 ns |     1,751.014 ns |     1,637.900 ns |  1.00 |    0.00 |   49.3164 |   15.3809 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     171,692.0 ns |     1,701.829 ns |     1,591.892 ns |  0.90 |    0.01 |   40.7715 |   11.7188 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  81,663,625.0 ns | 1,626,054.816 ns | 1,597,003.252 ns |  0.84 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 |  96,938,058.9 ns | 1,076,812.877 ns | 1,007,251.433 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.55 KB |
    | CisternLinq |           List |  False |     466544 |  71,966,388.3 ns |   660,846.272 ns |   618,156.012 ns |  0.74 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.06 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         889.4 ns |         6.014 ns |         5.331 ns |  0.53 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,665.4 ns |         9.624 ns |         9.002 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       1,687.7 ns |        19.877 ns |        18.593 ns |  1.01 |    0.01 |    0.6618 |         - |        - |     2.03 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      56,877.3 ns |       877.227 ns |       820.559 ns |  0.78 |    0.02 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      73,134.5 ns |     1,046.827 ns |       979.203 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      62,265.6 ns |       935.171 ns |       874.759 ns |  0.85 |    0.02 |   14.0381 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  39,488,792.8 ns |   627,315.679 ns |   586,791.476 ns |  0.71 |    0.02 | 2076.9231 |  846.1538 | 230.7692 | 11399.22 KB |
    |  SystemLinq |           List |   True |     466544 |  55,588,616.0 ns |   712,009.162 ns |   666,013.812 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16541.01 KB |
    | CisternLinq |           List |   True |     466544 |  42,888,458.3 ns |   678,613.368 ns |   634,775.365 ns |  0.77 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10730.41 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |         964.7 ns |         8.979 ns |         8.399 ns |  0.49 |    0.01 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       1,958.7 ns |        25.695 ns |        24.035 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       2,077.1 ns |        22.237 ns |        20.801 ns |  1.06 |    0.02 |    0.7629 |         - |        - |     2.34 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |      95,379.1 ns |     1,065.069 ns |       996.266 ns |  0.48 |    0.01 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     197,034.8 ns |     2,632.652 ns |     2,462.584 ns |  1.00 |    0.00 |   51.5137 |   17.0898 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     175,096.6 ns |     2,701.576 ns |     2,527.056 ns |  0.89 |    0.01 |   39.3066 |   12.4512 |        - |   137.89 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  81,488,733.0 ns |   584,738.957 ns |   488,283.512 ns |  0.82 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 |  99,281,440.0 ns | 1,116,975.212 ns | 1,044,819.306 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.99 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  70,844,506.7 ns | 1,055,194.577 ns |   987,029.662 ns |  0.71 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11649.15 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         809.2 ns |        11.747 ns |        10.988 ns |  0.49 |    0.01 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,659.2 ns |        24.068 ns |        22.513 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       1,784.6 ns |        27.753 ns |        25.960 ns |  1.08 |    0.03 |    0.6771 |         - |        - |     2.08 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      55,303.1 ns |       982.491 ns |       919.023 ns |  0.74 |    0.02 |   13.6108 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      74,821.6 ns |     1,041.919 ns |       974.611 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      60,327.3 ns |       844.258 ns |       789.719 ns |  0.81 |    0.01 |   14.0991 |         - |        - |     43.3 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  38,895,586.7 ns |   393,966.222 ns |   368,516.248 ns |  0.71 |    0.02 | 2076.9231 |  846.1538 | 230.7692 | 11399.42 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  54,732,380.0 ns |   972,428.111 ns |   909,609.858 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.56 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  42,892,354.4 ns |   429,706.522 ns |   401,947.748 ns |  0.78 |    0.02 | 2000.0000 |  846.1538 | 230.7692 | 10731.34 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |         914.4 ns |        11.262 ns |        10.534 ns |  0.46 |    0.01 |    0.6037 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       1,969.8 ns |        18.542 ns |        17.344 ns |  1.00 |    0.00 |    0.9384 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       2,031.9 ns |        32.808 ns |        30.689 ns |  1.03 |    0.02 |    0.7439 |         - |        - |     2.29 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |      92,787.4 ns |       876.785 ns |       777.247 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     188,815.8 ns |     2,595.772 ns |     2,428.087 ns |  1.00 |    0.00 |   49.0723 |   14.6484 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     165,253.6 ns |     2,027.388 ns |     1,896.420 ns |  0.88 |    0.02 |   40.2832 |   11.2305 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  81,495,662.9 ns | 1,556,240.333 ns | 1,455,708.173 ns |  0.82 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 |  99,053,805.6 ns | 1,148,031.746 ns | 1,073,869.607 ns |  1.00 |    0.00 | 3166.6667 | 1500.0000 | 500.0000 | 16540.55 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  66,282,927.5 ns | 1,096,354.016 ns | 1,025,530.226 ns |  0.67 |    0.02 | 2125.0000 | 1000.0000 | 250.0000 | 11647.84 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         766.8 ns |        14.310 ns |        14.054 ns |  0.48 |    0.01 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,607.4 ns |        18.312 ns |        17.129 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       1,624.4 ns |        11.711 ns |        10.382 ns |  1.01 |    0.01 |    0.6599 |         - |        - |     2.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      53,322.9 ns |       693.674 ns |       648.863 ns |  0.73 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      72,961.4 ns |       738.401 ns |       690.701 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      53,631.5 ns |       754.692 ns |       705.939 ns |  0.74 |    0.01 |   14.0991 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  38,273,656.4 ns |   460,633.023 ns |   430,876.416 ns |  0.69 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.02 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  55,232,645.0 ns |   515,114.024 ns |   456,635.165 ns |  1.00 |    0.00 | 3200.0000 | 1400.0000 | 500.0000 | 16541.36 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  40,330,614.9 ns |   525,101.392 ns |   491,180.168 ns |  0.73 |    0.01 | 2000.0000 |  846.1538 | 230.7692 | 10731.21 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       1,901.3 ns |        30.515 ns |        28.543 ns |  0.63 |    0.01 |    0.6142 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       3,003.6 ns |        43.911 ns |        41.074 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       3,068.3 ns |        35.972 ns |        33.648 ns |  1.02 |    0.01 |    0.7591 |         - |        - |     2.33 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     180,570.2 ns |     2,397.294 ns |     2,242.430 ns |  0.65 |    0.01 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     276,436.5 ns |     3,961.396 ns |     3,705.493 ns |  1.00 |    0.00 |   50.7813 |   13.6719 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     259,106.7 ns |     4,588.914 ns |     4,292.473 ns |  0.94 |    0.02 |   39.0625 |   12.2070 |        - |   137.86 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 127,094,761.7 ns | 1,907,540.740 ns | 1,784,314.791 ns |  0.91 |    0.02 | 1750.0000 |  750.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 140,332,766.7 ns | 2,044,909.693 ns | 1,912,809.796 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 112,467,678.7 ns | 1,965,822.050 ns | 1,838,831.165 ns |  0.80 |    0.02 | 1800.0000 |  800.0000 |        - | 11647.87 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,771.1 ns |        22.501 ns |        21.048 ns |  0.69 |    0.01 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,577.3 ns |        23.412 ns |        21.900 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       2,814.8 ns |        11.422 ns |        10.125 ns |  1.09 |    0.01 |    0.6714 |         - |        - |     2.06 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     132,674.5 ns |     1,710.394 ns |     1,599.904 ns |  0.86 |    0.01 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     154,101.9 ns |     1,906.664 ns |     1,783.495 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     138,839.2 ns |     1,629.494 ns |     1,524.230 ns |  0.90 |    0.01 |   13.9160 |         - |        - |    43.29 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  72,898,504.4 ns |   842,040.093 ns |   703,141.614 ns |  0.83 |    0.02 | 1857.1429 |  714.2857 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  87,855,275.5 ns | 1,711,227.841 ns | 1,757,305.004 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 | 16540.89 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  74,198,024.6 ns | 1,424,385.404 ns | 1,524,076.424 ns |  0.85 |    0.02 | 1857.1429 |  714.2857 | 142.8571 | 10730.37 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |         914.9 ns |         9.853 ns |         9.217 ns |  0.47 |    0.00 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       1,951.4 ns |        17.773 ns |        16.625 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,270.7 ns |        33.786 ns |        31.603 ns |  1.16 |    0.02 |    0.7439 |         - |        - |     2.29 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |      96,424.3 ns |     1,236.186 ns |     1,156.329 ns |  0.50 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     193,151.0 ns |     3,717.594 ns |     3,651.175 ns |  1.00 |    0.00 |   50.5371 |   13.4277 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     175,376.9 ns |     3,054.863 ns |     2,857.521 ns |  0.91 |    0.03 |   39.5508 |   12.6953 |        - |   137.82 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  81,409,122.4 ns |   920,040.300 ns |   815,591.762 ns |  0.83 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 |  98,409,800.0 ns | 1,260,728.768 ns | 1,179,286.471 ns |  1.00 |    0.00 | 2800.0000 | 1400.0000 | 200.0000 | 16540.67 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  71,615,652.5 ns | 1,108,772.650 ns | 1,037,146.624 ns |  0.73 |    0.01 | 2125.0000 | 1000.0000 | 250.0000 | 11648.45 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         780.9 ns |         9.529 ns |         8.914 ns |  0.47 |    0.01 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,673.3 ns |        24.591 ns |        23.002 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       1,806.2 ns |         9.162 ns |         8.122 ns |  1.08 |    0.02 |    0.6599 |         - |        - |     2.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      55,468.3 ns |       591.512 ns |       553.301 ns |  0.74 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      74,869.3 ns |     1,061.653 ns |       993.071 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      64,314.9 ns |       954.644 ns |       892.975 ns |  0.86 |    0.01 |   14.0381 |         - |        - |    43.25 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  40,060,622.1 ns |   725,351.982 ns |   678,494.694 ns |  0.71 |    0.02 | 2000.0000 |  846.1538 | 153.8462 | 11399.17 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  56,143,840.0 ns | 1,120,685.028 ns | 1,100,662.546 ns |  1.00 |    0.00 | 3000.0000 | 1300.0000 | 300.0000 |  16540.3 KB |
    | CisternLinq |     FSharpList |   True |     466544 |  41,702,124.4 ns |   476,471.011 ns |   445,691.280 ns |  0.74 |    0.02 | 1916.6667 |  750.0000 | 166.6667 | 10730.39 KB |
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
