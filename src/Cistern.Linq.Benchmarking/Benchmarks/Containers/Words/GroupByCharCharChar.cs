using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    /*
    |      Method |  ContainerType | Sorted | WordsCount |             Mean |            Error |           StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
    |------------ |--------------- |------- |----------- |-----------------:|-----------------:|-----------------:|------:|--------:|----------:|----------:|---------:|------------:|
    |     ForLoop |          Array |  False |         10 |       1,061.9 ns |         7.914 ns |         7.016 ns |  0.51 |    0.01 |    0.6027 |         - |        - |     1.85 KB |
    |  SystemLinq |          Array |  False |         10 |       2,067.6 ns |        17.870 ns |        16.716 ns |  1.00 |    0.00 |    0.9270 |         - |        - |     2.84 KB |
    | CisternLinq |          Array |  False |         10 |       1,754.0 ns |         7.862 ns |         6.565 ns |  0.85 |    0.01 |    0.7648 |         - |        - |     2.34 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |       1000 |     102,370.0 ns |       587.173 ns |       549.242 ns |  0.49 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq |          Array |  False |       1000 |     208,638.5 ns |     3,061.656 ns |     2,863.875 ns |  1.00 |    0.00 |   49.8047 |   14.1602 |        - |   176.24 KB |
    | CisternLinq |          Array |  False |       1000 |     165,123.3 ns |     1,197.840 ns |     1,120.460 ns |  0.79 |    0.01 |   43.4570 |   12.6953 |        - |   137.95 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |  False |     466544 |  89,061,737.8 ns |   593,155.306 ns |   554,837.841 ns |  0.87 |    0.01 | 1833.3333 |  833.3333 |        - | 11398.83 KB |
    |  SystemLinq |          Array |  False |     466544 | 102,035,673.3 ns |   987,139.653 ns |   923,371.044 ns |  1.00 |    0.00 | 3000.0000 | 1400.0000 | 400.0000 | 16541.41 KB |
    | CisternLinq |          Array |  False |     466544 |  76,966,875.2 ns |   686,210.037 ns |   641,881.295 ns |  0.75 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11647.95 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |         10 |         897.9 ns |         6.377 ns |         5.965 ns |  0.50 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq |          Array |   True |         10 |       1,800.3 ns |         8.561 ns |         7.149 ns |  1.00 |    0.00 |    0.8297 |         - |        - |     2.55 KB |
    | CisternLinq |          Array |   True |         10 |       1,491.5 ns |         9.484 ns |         8.408 ns |  0.83 |    0.01 |    0.6771 |         - |        - |     2.08 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |       1000 |      60,003.1 ns |       367.564 ns |       343.819 ns |  0.77 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq |          Array |   True |       1000 |      77,531.9 ns |       475.249 ns |       444.548 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.59 KB |
    | CisternLinq |          Array |   True |       1000 |      62,569.5 ns |       340.607 ns |       301.939 ns |  0.81 |    0.00 |   14.0381 |         - |        - |    43.38 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |          Array |   True |     466544 |  41,790,367.2 ns |   412,693.642 ns |   386,033.889 ns |  0.73 |    0.01 | 2076.9231 |  846.1538 | 230.7692 |    11399 KB |
    |  SystemLinq |          Array |   True |     466544 |  57,414,221.5 ns |   796,016.154 ns |   744,594.005 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.97 KB |
    | CisternLinq |          Array |   True |     466544 |  44,148,740.0 ns |   352,191.933 ns |   329,440.552 ns |  0.77 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10731.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |         10 |       1,081.5 ns |         8.478 ns |         7.930 ns |  0.50 |    0.00 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |           List |  False |         10 |       2,175.4 ns |        15.689 ns |        13.908 ns |  1.00 |    0.00 |    0.9346 |         - |        - |     2.87 KB |
    | CisternLinq |           List |  False |         10 |       1,949.2 ns |        17.555 ns |        16.421 ns |  0.90 |    0.01 |    0.7706 |         - |        - |     2.37 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |       1000 |     109,072.8 ns |       867.634 ns |       811.585 ns |  0.51 |    0.01 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |           List |  False |       1000 |     214,866.7 ns |     2,084.862 ns |     1,848.176 ns |  1.00 |    0.00 |   49.0723 |   14.4043 |        - |   176.27 KB |
    | CisternLinq |           List |  False |       1000 |     174,813.7 ns |     1,328.684 ns |     1,242.852 ns |  0.81 |    0.01 |   42.7246 |   10.7422 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |  False |     466544 |  91,898,358.9 ns | 1,157,957.521 ns | 1,083,154.184 ns |  0.87 |    0.01 | 1833.3333 |  833.3333 |        - | 11398.84 KB |
    |  SystemLinq |           List |  False |     466544 | 105,893,517.3 ns |   885,739.650 ns |   828,521.418 ns |  1.00 |    0.00 | 3000.0000 | 1400.0000 | 400.0000 | 16541.53 KB |
    | CisternLinq |           List |  False |     466544 |  83,875,544.8 ns |   511,208.513 ns |   478,184.760 ns |  0.79 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 |  11648.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |         10 |         901.3 ns |         5.654 ns |         5.288 ns |  0.49 |    0.00 |    0.5503 |         - |        - |     1.69 KB |
    |  SystemLinq |           List |   True |         10 |       1,843.7 ns |        11.357 ns |        10.067 ns |  1.00 |    0.00 |    0.8373 |         - |        - |     2.57 KB |
    | CisternLinq |           List |   True |         10 |       1,607.4 ns |        10.999 ns |        10.288 ns |  0.87 |    0.01 |    0.6847 |         - |        - |      2.1 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |       1000 |      64,460.6 ns |       405.636 ns |       359.586 ns |  0.78 |    0.00 |   13.5498 |         - |        - |    41.73 KB |
    |  SystemLinq |           List |   True |       1000 |      82,262.9 ns |       588.964 ns |       550.917 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.62 KB |
    | CisternLinq |           List |   True |       1000 |      71,886.2 ns |       504.704 ns |       472.101 ns |  0.87 |    0.01 |   14.1602 |         - |        - |    43.41 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |           List |   True |     466544 |  43,280,286.7 ns |   441,857.591 ns |   413,313.865 ns |  0.73 |    0.01 | 2083.3333 |  833.3333 | 250.0000 | 11399.79 KB |
    |  SystemLinq |           List |   True |     466544 |  59,647,593.3 ns |   551,467.116 ns |   515,842.682 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.97 KB |
    | CisternLinq |           List |   True |     466544 |  46,719,306.1 ns |   556,568.878 ns |   520,614.874 ns |  0.78 |    0.01 | 1909.0909 |  727.2727 | 181.8182 | 10730.34 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |         10 |       1,053.1 ns |         8.108 ns |         7.584 ns |  0.48 |    0.01 |    0.6142 |         - |        - |     1.88 KB |
    |  SystemLinq |     Enumerable |  False |         10 |       2,205.2 ns |        17.023 ns |        15.924 ns |  1.00 |    0.00 |    0.9499 |         - |        - |     2.91 KB |
    | CisternLinq |     Enumerable |  False |         10 |       1,969.6 ns |        15.955 ns |        14.925 ns |  0.89 |    0.01 |    0.7858 |         - |        - |     2.41 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |       1000 |     106,369.9 ns |       545.465 ns |       510.228 ns |  0.49 |    0.00 |   34.4238 |   11.4746 |        - |   105.82 KB |
    |  SystemLinq |     Enumerable |  False |       1000 |     216,492.3 ns |     1,659.973 ns |     1,552.739 ns |  1.00 |    0.00 |   51.0254 |   15.1367 |        - |   176.31 KB |
    | CisternLinq |     Enumerable |  False |       1000 |     175,328.3 ns |     1,857.068 ns |     1,737.103 ns |  0.81 |    0.01 |   43.4570 |    9.7656 |        - |   138.04 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |  False |     466544 |  90,965,503.3 ns |   979,018.446 ns |   915,774.462 ns |  0.85 |    0.01 | 1833.3333 |  833.3333 |        - | 11398.86 KB |
    |  SystemLinq |     Enumerable |  False |     466544 | 107,046,349.3 ns |   930,677.776 ns |   870,556.569 ns |  1.00 |    0.00 | 3000.0000 | 1400.0000 | 400.0000 | 16541.68 KB |
    | CisternLinq |     Enumerable |  False |     466544 |  83,145,141.9 ns |   518,666.634 ns |   485,161.091 ns |  0.78 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 |  11649.3 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |         10 |         907.5 ns |         5.687 ns |         5.042 ns |  0.48 |    0.00 |    0.5579 |         - |        - |     1.71 KB |
    |  SystemLinq |     Enumerable |   True |         10 |       1,874.2 ns |         9.740 ns |         9.111 ns |  1.00 |    0.00 |    0.8526 |         - |        - |     2.62 KB |
    | CisternLinq |     Enumerable |   True |         10 |       1,652.1 ns |        13.220 ns |        12.366 ns |  0.88 |    0.01 |    0.7000 |         - |        - |     2.15 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |       1000 |      61,776.6 ns |       472.852 ns |       442.306 ns |  0.75 |    0.01 |   13.5498 |         - |        - |    41.76 KB |
    |  SystemLinq |     Enumerable |   True |       1000 |      82,861.8 ns |       784.374 ns |       733.704 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.66 KB |
    | CisternLinq |     Enumerable |   True |       1000 |      70,169.8 ns |       508.856 ns |       475.984 ns |  0.85 |    0.01 |   14.1602 |         - |        - |    43.46 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     Enumerable |   True |     466544 |  42,204,041.1 ns |   378,947.847 ns |   354,468.051 ns |  0.71 |    0.01 | 2083.3333 |  833.3333 | 250.0000 | 11399.28 KB |
    |  SystemLinq |     Enumerable |   True |     466544 |  59,795,437.8 ns |   698,645.943 ns |   653,513.848 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16541.26 KB |
    | CisternLinq |     Enumerable |   True |     466544 |  47,982,044.4 ns |   527,735.128 ns |   493,643.766 ns |  0.80 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10731.26 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |         10 |       1,038.7 ns |         6.695 ns |         6.262 ns |  0.48 |    0.00 |    0.6027 |         - |        - |     1.85 KB |
    |  SystemLinq | ImmutableArray |  False |         10 |       2,171.4 ns |        12.148 ns |        11.364 ns |  1.00 |    0.00 |    0.9384 |         - |        - |     2.88 KB |
    | CisternLinq | ImmutableArray |  False |         10 |       1,809.9 ns |        13.670 ns |        12.787 ns |  0.83 |    0.00 |    0.7687 |         - |        - |     2.36 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |       1000 |     104,343.3 ns |       417.172 ns |       369.812 ns |  0.48 |    0.01 |   34.4238 |   11.4746 |        - |   105.79 KB |
    |  SystemLinq | ImmutableArray |  False |       1000 |     216,138.9 ns |     2,574.255 ns |     2,407.960 ns |  1.00 |    0.00 |   49.0723 |   14.1602 |        - |   176.28 KB |
    | CisternLinq | ImmutableArray |  False |       1000 |     166,449.1 ns |     1,453.589 ns |     1,359.688 ns |  0.77 |    0.01 |   42.7246 |   10.2539 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |  False |     466544 |  89,628,240.0 ns |   773,648.456 ns |   723,671.246 ns |  0.84 |    0.01 | 1833.3333 |  833.3333 |        - | 11398.83 KB |
    |  SystemLinq | ImmutableArray |  False |     466544 | 106,751,378.7 ns | 1,038,543.526 ns |   971,454.259 ns |  1.00 |    0.00 | 3000.0000 | 1400.0000 | 400.0000 | 16541.21 KB |
    | CisternLinq | ImmutableArray |  False |     466544 |  74,936,346.7 ns |   712,015.043 ns |   666,019.313 ns |  0.70 |    0.01 | 2142.8571 | 1000.0000 | 285.7143 | 11648.76 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |         10 |         886.4 ns |         5.698 ns |         5.330 ns |  0.47 |    0.00 |    0.5484 |         - |        - |     1.68 KB |
    |  SystemLinq | ImmutableArray |   True |         10 |       1,885.5 ns |        12.838 ns |        12.009 ns |  1.00 |    0.00 |    0.8430 |         - |        - |     2.59 KB |
    | CisternLinq | ImmutableArray |   True |         10 |       1,577.8 ns |         9.924 ns |         9.283 ns |  0.84 |    0.01 |    0.6828 |         - |        - |     2.09 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |       1000 |      59,327.4 ns |       469.731 ns |       439.387 ns |  0.72 |    0.01 |   13.6108 |         - |        - |    41.73 KB |
    |  SystemLinq | ImmutableArray |   True |       1000 |      82,071.1 ns |       690.549 ns |       645.940 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.63 KB |
    | CisternLinq | ImmutableArray |   True |       1000 |      62,718.1 ns |       231.018 ns |       204.792 ns |  0.76 |    0.01 |   14.1602 |         - |        - |     43.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop | ImmutableArray |   True |     466544 |  41,273,060.5 ns |   230,275.010 ns |   215,399.387 ns |  0.70 |    0.01 | 2076.9231 |  846.1538 | 230.7692 | 11399.37 KB |
    |  SystemLinq | ImmutableArray |   True |     466544 |  59,176,488.9 ns |   387,148.181 ns |   362,138.649 ns |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 | 16540.52 KB |
    | CisternLinq | ImmutableArray |   True |     466544 |  44,419,008.9 ns |   525,271.732 ns |   491,339.504 ns |  0.75 |    0.01 | 2000.0000 |  833.3333 | 250.0000 | 10730.64 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |         10 |       2,092.6 ns |        18.381 ns |        17.193 ns |  0.63 |    0.01 |    0.6142 |         - |        - |     1.89 KB |
    |  SystemLinq |  ImmutableList |  False |         10 |       3,304.6 ns |        28.282 ns |        26.455 ns |  1.00 |    0.00 |    0.9537 |         - |        - |     2.92 KB |
    | CisternLinq |  ImmutableList |  False |         10 |       3,141.9 ns |        16.151 ns |        15.107 ns |  0.95 |    0.01 |    0.7820 |         - |        - |      2.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |       1000 |     199,115.9 ns |     1,508.229 ns |     1,410.799 ns |  0.63 |    0.01 |   34.4238 |   11.4746 |        - |   105.83 KB |
    |  SystemLinq |  ImmutableList |  False |       1000 |     316,437.4 ns |     2,469.933 ns |     2,310.377 ns |  1.00 |    0.00 |   47.3633 |   14.6484 |        - |   176.32 KB |
    | CisternLinq |  ImmutableList |  False |       1000 |     265,465.7 ns |     1,940.380 ns |     1,815.032 ns |  0.84 |    0.01 |   43.4570 |   11.7188 |        - |   138.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |  False |     466544 | 140,863,308.3 ns |   821,355.806 ns |   768,296.731 ns |  0.89 |    0.01 | 1750.0000 |  750.0000 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |  False |     466544 | 158,377,173.3 ns | 1,124,634.934 ns | 1,051,984.215 ns |  1.00 |    0.00 | 2750.0000 | 1250.0000 |        - | 16540.19 KB |
    | CisternLinq |  ImmutableList |  False |     466544 | 132,583,926.7 ns |   910,758.368 ns |   851,923.943 ns |  0.84 |    0.01 | 2000.0000 | 1000.0000 |        - | 11648.02 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |         10 |       1,967.6 ns |        18.083 ns |        16.915 ns |  0.67 |    0.00 |    0.5608 |         - |        - |     1.72 KB |
    |  SystemLinq |  ImmutableList |   True |         10 |       2,929.1 ns |        13.759 ns |        12.870 ns |  1.00 |    0.00 |    0.8545 |         - |        - |     2.63 KB |
    | CisternLinq |  ImmutableList |   True |         10 |       2,909.5 ns |        22.427 ns |        20.978 ns |  0.99 |    0.01 |    0.6943 |         - |        - |     2.13 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |       1000 |     148,294.9 ns |     1,218.742 ns |     1,140.012 ns |  0.85 |    0.01 |   13.4277 |         - |        - |    41.77 KB |
    |  SystemLinq |  ImmutableList |   True |       1000 |     174,577.4 ns |     1,072.937 ns |     1,003.626 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.67 KB |
    | CisternLinq |  ImmutableList |   True |       1000 |     169,359.7 ns |     1,133.882 ns |     1,060.634 ns |  0.97 |    0.01 |   14.1602 |         - |        - |    43.44 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |  ImmutableList |   True |     466544 |  83,083,362.9 ns |   678,532.214 ns |   634,699.454 ns |  0.85 |    0.01 | 1857.1429 |  714.2857 |        - | 11398.87 KB |
    |  SystemLinq |  ImmutableList |   True |     466544 |  97,814,738.9 ns |   846,400.549 ns |   791,723.599 ns |  1.00 |    0.00 | 2833.3333 | 1166.6667 | 166.6667 | 16541.38 KB |
    | CisternLinq |  ImmutableList |   True |     466544 |  88,877,274.4 ns | 1,253,718.569 ns | 1,172,729.127 ns |  0.91 |    0.02 | 2000.0000 |  833.3333 | 166.6667 | 10730.78 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |         10 |       1,118.8 ns |         6.783 ns |         6.345 ns |  0.50 |    0.01 |    0.6065 |         - |        - |     1.86 KB |
    |  SystemLinq |     FSharpList |  False |         10 |       2,245.0 ns |        18.556 ns |        17.358 ns |  1.00 |    0.00 |    0.9422 |         - |        - |     2.89 KB |
    | CisternLinq |     FSharpList |  False |         10 |       2,010.0 ns |        26.139 ns |        21.827 ns |  0.90 |    0.02 |    0.7668 |         - |        - |     2.36 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |       1000 |     108,276.1 ns |       653.335 ns |       611.130 ns |  0.49 |    0.00 |   34.4238 |   11.4746 |        - |    105.8 KB |
    |  SystemLinq |     FSharpList |  False |       1000 |     220,167.6 ns |     2,083.148 ns |     1,948.578 ns |  1.00 |    0.00 |   49.8047 |   12.6953 |        - |   176.29 KB |
    | CisternLinq |     FSharpList |  False |       1000 |     184,994.0 ns |     1,398.153 ns |     1,239.426 ns |  0.84 |    0.01 |   43.2129 |   10.7422 |        - |   137.98 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |  False |     466544 |  93,639,868.9 ns |   829,280.496 ns |   775,709.491 ns |  0.84 |    0.01 | 1833.3333 |  833.3333 |        - | 11398.84 KB |
    |  SystemLinq |     FSharpList |  False |     466544 | 111,999,358.7 ns |   967,075.673 ns |   904,603.184 ns |  1.00 |    0.00 | 2800.0000 | 1400.0000 | 200.0000 | 16540.62 KB |
    | CisternLinq |     FSharpList |  False |     466544 |  81,479,011.1 ns | 1,200,329.051 ns | 1,122,788.539 ns |  0.73 |    0.01 | 2000.0000 | 1000.0000 | 166.6667 | 11648.97 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |         10 |         949.5 ns |         6.148 ns |         5.751 ns |  0.50 |    0.00 |    0.5493 |         - |        - |     1.69 KB |
    |  SystemLinq |     FSharpList |   True |         10 |       1,893.8 ns |        13.403 ns |        11.882 ns |  1.00 |    0.00 |    0.8450 |         - |        - |     2.59 KB |
    | CisternLinq |     FSharpList |   True |         10 |       1,696.3 ns |         8.839 ns |         8.268 ns |  0.90 |    0.01 |    0.6828 |         - |        - |     2.09 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |       1000 |      63,794.2 ns |       470.321 ns |       439.938 ns |  0.75 |    0.01 |   13.5498 |         - |        - |    41.73 KB |
    |  SystemLinq |     FSharpList |   True |       1000 |      85,554.6 ns |       429.376 ns |       401.638 ns |  1.00 |    0.00 |   20.0195 |         - |        - |    61.64 KB |
    | CisternLinq |     FSharpList |   True |       1000 |      75,394.5 ns |       344.821 ns |       322.546 ns |  0.88 |    0.01 |   14.1602 |         - |        - |     43.4 KB |
    |             |                |        |            |                  |                  |                  |       |         |           |           |          |             |
    |     ForLoop |     FSharpList |   True |     466544 |  44,391,026.1 ns |   458,618.012 ns |   428,991.573 ns |  0.72 |    0.01 | 2000.0000 |  833.3333 | 166.6667 | 11399.15 KB |
    |  SystemLinq |     FSharpList |   True |     466544 |  62,018,483.0 ns |   824,091.230 ns |   730,535.410 ns |  1.00 |    0.00 | 3000.0000 | 1250.0000 | 250.0000 | 16541.23 KB |
    | CisternLinq |     FSharpList |   True |     466544 |  47,057,973.9 ns |   471,232.160 ns |   440,790.854 ns |  0.76 |    0.01 | 1909.0909 |  727.2727 | 181.8182 | 10731.04 KB |
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
