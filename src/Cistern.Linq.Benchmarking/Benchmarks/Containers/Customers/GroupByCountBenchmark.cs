using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |         Mean |       Error |      StdDev |       Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |-------------:|------------:|------------:|-------------:|------:|--------:|-------:|------:|------:|----------:|
    |     ForLoop |          Array |             0 |     108.3 ns |   0.6383 ns |   0.5970 ns |     108.2 ns |  0.73 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |          Array |             0 |     148.6 ns |   1.9361 ns |   1.8110 ns |     149.3 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |          Array |             0 |     376.8 ns |   1.4495 ns |   1.3559 ns |     376.2 ns |  2.54 |    0.03 | 0.1988 |     - |     - |     624 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |          Array |             1 |     211.4 ns |   1.6112 ns |   1.5071 ns |     211.4 ns |  0.71 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq |          Array |             1 |     295.8 ns |   3.1486 ns |   2.9452 ns |     295.9 ns |  1.00 |    0.00 | 0.1655 |     - |     - |     520 B |
    | CisternLinq |          Array |             1 |     647.0 ns |   4.5659 ns |   4.2710 ns |     647.7 ns |  2.19 |    0.02 | 0.2346 |     - |     - |     736 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |          Array |            10 |     838.9 ns |   5.4958 ns |   5.1408 ns |     839.4 ns |  0.66 |    0.00 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq |          Array |            10 |   1,279.4 ns |   5.6041 ns |   5.2421 ns |   1,278.9 ns |  1.00 |    0.00 | 0.4196 |     - |     - |    1320 B |
    | CisternLinq |          Array |            10 |   1,552.8 ns |  18.1214 ns |  16.9508 ns |   1,546.5 ns |  1.21 |    0.02 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |          Array |           100 |   5,188.1 ns |  20.9611 ns |  19.6070 ns |   5,190.3 ns |  0.71 |    0.00 | 0.4959 |     - |     - |    1568 B |
    |  SystemLinq |          Array |           100 |   7,328.4 ns |  36.0394 ns |  33.7113 ns |   7,337.8 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq |          Array |           100 |   6,727.9 ns | 135.2317 ns | 214.4918 ns |   6,696.1 ns |  0.92 |    0.03 | 1.1063 |     - |     - |    3504 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |          Array |          1000 |  48,434.9 ns | 648.8167 ns | 606.9036 ns |  48,422.2 ns |  0.86 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq |          Array |          1000 |  56,443.9 ns | 464.0419 ns | 434.0651 ns |  56,530.7 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27800 B |
    | CisternLinq |          Array |          1000 |  32,877.8 ns | 518.2722 ns | 484.7921 ns |  32,891.3 ns |  0.58 |    0.01 | 4.6997 |     - |     - |   14800 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |           List |             0 |     130.2 ns |   2.5950 ns |   2.8843 ns |     130.0 ns |  0.75 |    0.03 | 0.0455 |     - |     - |     144 B |
    |  SystemLinq |           List |             0 |     173.8 ns |   3.4812 ns |   3.2564 ns |     174.1 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |           List |             0 |     423.2 ns |   8.4162 ns |  13.3490 ns |     424.8 ns |  2.48 |    0.08 | 0.1984 |     - |     - |     624 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |           List |             1 |     250.9 ns |   4.5322 ns |   4.2394 ns |     251.3 ns |  0.72 |    0.02 | 0.1168 |     - |     - |     368 B |
    |  SystemLinq |           List |             1 |     348.5 ns |   5.5104 ns |   5.1544 ns |     350.6 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |           List |             1 |     712.2 ns |  13.8741 ns |  15.4210 ns |     713.0 ns |  2.05 |    0.04 | 0.2337 |     - |     - |     736 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |           List |            10 |     977.2 ns |  17.1455 ns |  16.0380 ns |     984.2 ns |  0.67 |    0.01 | 0.2422 |     - |     - |     768 B |
    |  SystemLinq |           List |            10 |   1,448.9 ns |  11.3950 ns |  10.1014 ns |   1,446.9 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |           List |            10 |   1,831.3 ns |  17.6251 ns |  15.6242 ns |   1,832.1 ns |  1.26 |    0.01 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |           List |           100 |   5,983.6 ns | 129.6784 ns | 310.7011 ns |   5,810.3 ns |  0.81 |    0.02 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |           100 |   7,951.2 ns |  42.8137 ns |  40.0480 ns |   7,947.5 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |           List |           100 |   7,617.3 ns |  37.0221 ns |  34.6305 ns |   7,609.1 ns |  0.96 |    0.01 | 1.0986 |     - |     - |    3504 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |           List |          1000 |  50,299.0 ns | 208.8257 ns | 195.3357 ns |  50,292.7 ns |  0.89 |    0.01 | 0.4883 |     - |     - |    1576 B |
    |  SystemLinq |           List |          1000 |  56,415.2 ns | 346.5447 ns | 324.1581 ns |  56,296.9 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27808 B |
    | CisternLinq |           List |          1000 |  40,736.0 ns | 301.2833 ns | 281.8206 ns |  40,782.9 ns |  0.72 |    0.01 | 4.6387 |     - |     - |   14800 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             0 |     128.1 ns |   0.9270 ns |   0.8671 ns |     128.0 ns |  0.79 |    0.01 | 0.0534 |     - |     - |     168 B |
    |  SystemLinq |     Enumerable |             0 |     163.0 ns |   1.4202 ns |   1.3284 ns |     163.3 ns |  1.00 |    0.00 | 0.1197 |     - |     - |     376 B |
    | CisternLinq |     Enumerable |             0 |     489.6 ns |   3.2006 ns |   2.9938 ns |     489.6 ns |  3.00 |    0.03 | 0.2193 |     - |     - |     688 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |             1 |     219.0 ns |   1.3999 ns |   1.3095 ns |     218.5 ns |  0.73 |    0.01 | 0.1247 |     - |     - |     392 B |
    |  SystemLinq |     Enumerable |             1 |     299.3 ns |   2.2910 ns |   2.1430 ns |     298.8 ns |  1.00 |    0.00 | 0.1760 |     - |     - |     552 B |
    | CisternLinq |     Enumerable |             1 |     765.7 ns |   5.0473 ns |   4.7213 ns |     766.8 ns |  2.56 |    0.02 | 0.2546 |     - |     - |     800 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |            10 |     870.3 ns |   7.0500 ns |   6.5946 ns |     871.2 ns |  0.67 |    0.01 | 0.2518 |     - |     - |     792 B |
    |  SystemLinq |     Enumerable |            10 |   1,293.4 ns |   8.1071 ns |   7.5834 ns |   1,292.7 ns |  1.00 |    0.00 | 0.4292 |     - |     - |    1352 B |
    | CisternLinq |     Enumerable |            10 |   1,695.2 ns |  12.6684 ns |  11.8500 ns |   1,697.6 ns |  1.31 |    0.01 | 0.4768 |     - |     - |    1496 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |           100 |   5,406.2 ns |  39.9230 ns |  37.3440 ns |   5,389.0 ns |  0.71 |    0.01 | 0.5035 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |           100 |   7,611.6 ns |  56.1881 ns |  49.8093 ns |   7,605.3 ns |  1.00 |    0.00 | 1.6632 |     - |     - |    5224 B |
    | CisternLinq |     Enumerable |           100 |   7,319.2 ns |  35.5059 ns |  33.2123 ns |   7,318.8 ns |  0.96 |    0.01 | 1.1292 |     - |     - |    3568 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     Enumerable |          1000 |  48,207.5 ns | 217.3021 ns | 203.2645 ns |  48,151.9 ns |  0.86 |    0.01 | 0.4272 |     - |     - |    1600 B |
    |  SystemLinq |     Enumerable |          1000 |  56,040.9 ns | 234.1238 ns | 218.9995 ns |  56,003.9 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27832 B |
    | CisternLinq |     Enumerable |          1000 |  37,813.4 ns | 293.8853 ns | 274.9005 ns |  37,804.1 ns |  0.67 |    0.01 | 4.6387 |     - |     - |   14864 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             0 |     113.3 ns |   0.7109 ns |   0.6650 ns |     113.3 ns |  0.73 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq | ImmutableArray |             0 |     155.7 ns |   1.2120 ns |   1.0744 ns |     155.7 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq | ImmutableArray |             0 |     504.7 ns |   4.2817 ns |   4.0051 ns |     503.8 ns |  3.24 |    0.03 | 0.1974 |     - |     - |     624 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |             1 |     213.2 ns |   1.1998 ns |   1.1223 ns |     213.1 ns |  0.73 |    0.01 | 0.1144 |     - |     - |     360 B |
    |  SystemLinq | ImmutableArray |             1 |     290.9 ns |   3.3733 ns |   2.8169 ns |     290.3 ns |  1.00 |    0.00 | 0.1650 |     - |     - |     520 B |
    | CisternLinq | ImmutableArray |             1 |     751.6 ns |   4.1059 ns |   3.8407 ns |     751.9 ns |  2.59 |    0.03 | 0.2337 |     - |     - |     736 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |            10 |     843.6 ns |   7.3783 ns |   6.9017 ns |     840.9 ns |  0.65 |    0.01 | 0.2422 |     - |     - |     760 B |
    |  SystemLinq | ImmutableArray |            10 |   1,290.3 ns |  10.1957 ns |   9.5371 ns |   1,292.4 ns |  1.00 |    0.00 | 0.4177 |     - |     - |    1320 B |
    | CisternLinq | ImmutableArray |            10 |   1,618.8 ns |   7.0919 ns |   6.6338 ns |   1,621.8 ns |  1.25 |    0.01 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |           100 |   5,281.1 ns |  40.5296 ns |  37.9114 ns |   5,287.6 ns |  0.72 |    0.01 | 0.4883 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |           100 |   7,288.5 ns |  63.8446 ns |  59.7202 ns |   7,312.5 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5192 B |
    | CisternLinq | ImmutableArray |           100 |   6,493.1 ns |  45.2469 ns |  42.3240 ns |   6,486.0 ns |  0.89 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop | ImmutableArray |          1000 |  46,160.9 ns | 142.2523 ns | 118.7871 ns |  46,185.8 ns |  0.85 |    0.00 | 0.4272 |     - |     - |    1568 B |
    |  SystemLinq | ImmutableArray |          1000 |  54,051.7 ns | 370.4048 ns | 346.4769 ns |  53,980.7 ns |  1.00 |    0.00 | 8.7891 |     - |     - |   27800 B |
    | CisternLinq | ImmutableArray |          1000 |  31,306.9 ns | 200.1301 ns | 187.2018 ns |  31,359.6 ns |  0.58 |    0.01 | 4.6387 |     - |     - |   14801 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             0 |     111.4 ns |   0.5804 ns |   0.5429 ns |     111.4 ns |  0.72 |    0.01 | 0.0330 |     - |     - |     104 B |
    |  SystemLinq |  ImmutableList |             0 |     155.6 ns |   1.3976 ns |   1.3073 ns |     155.6 ns |  1.00 |    0.00 | 0.0992 |     - |     - |     312 B |
    | CisternLinq |  ImmutableList |             0 |     620.7 ns |   4.2742 ns |   3.9981 ns |     620.0 ns |  3.99 |    0.04 | 0.1974 |     - |     - |     624 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |             1 |     572.1 ns |   4.9303 ns |   4.6118 ns |     571.6 ns |  0.91 |    0.01 | 0.1268 |     - |     - |     400 B |
    |  SystemLinq |  ImmutableList |             1 |     627.5 ns |   3.3563 ns |   2.8027 ns |     628.3 ns |  1.00 |    0.00 | 0.1774 |     - |     - |     560 B |
    | CisternLinq |  ImmutableList |             1 |   1,170.1 ns |   6.9872 ns |   6.5358 ns |   1,171.4 ns |  1.87 |    0.02 | 0.2327 |     - |     - |     736 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |            10 |   2,035.9 ns |  16.2642 ns |  15.2135 ns |   2,033.9 ns |  0.84 |    0.01 | 0.2518 |     - |     - |     800 B |
    |  SystemLinq |  ImmutableList |            10 |   2,417.7 ns |  12.0231 ns |  11.2465 ns |   2,419.0 ns |  1.00 |    0.00 | 0.4272 |     - |     - |    1360 B |
    | CisternLinq |  ImmutableList |            10 |   2,965.3 ns |  25.9674 ns |  24.2899 ns |   2,975.1 ns |  1.23 |    0.01 | 0.4539 |     - |     - |    1432 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |           100 |  15,656.4 ns | 103.2697 ns |  96.5985 ns |  15,687.0 ns |  0.89 |    0.01 | 0.4578 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |           100 |  17,570.7 ns |  94.8488 ns |  88.7216 ns |  17,567.6 ns |  1.00 |    0.00 | 1.6174 |     - |     - |    5232 B |
    | CisternLinq |  ImmutableList |           100 |  15,898.5 ns | 144.5461 ns | 135.2085 ns |  15,891.3 ns |  0.90 |    0.01 | 1.0986 |     - |     - |    3504 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |  ImmutableList |          1000 | 143,052.2 ns | 793.1773 ns | 741.9386 ns | 143,119.9 ns |  0.95 |    0.01 | 0.2441 |     - |     - |    1608 B |
    |  SystemLinq |  ImmutableList |          1000 | 150,740.5 ns | 898.3072 ns | 840.2771 ns | 150,825.9 ns |  1.00 |    0.00 | 8.5449 |     - |     - |   27840 B |
    | CisternLinq |  ImmutableList |          1000 | 123,632.1 ns | 679.1733 ns | 635.2991 ns | 123,767.4 ns |  0.82 |    0.01 | 4.6387 |     - |     - |   14801 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             0 |     124.7 ns |   0.7375 ns |   0.6538 ns |     124.7 ns |  0.79 |    0.01 | 0.0455 |     - |     - |     144 B |
    |  SystemLinq |     FSharpList |             0 |     158.2 ns |   1.3334 ns |   1.2473 ns |     158.6 ns |  1.00 |    0.00 | 0.1118 |     - |     - |     352 B |
    | CisternLinq |     FSharpList |             0 |     537.4 ns |   3.1034 ns |   2.9029 ns |     537.2 ns |  3.40 |    0.04 | 0.1984 |     - |     - |     624 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |             1 |     217.1 ns |   1.5907 ns |   1.4879 ns |     216.8 ns |  0.74 |    0.01 | 0.1173 |     - |     - |     368 B |
    |  SystemLinq |     FSharpList |             1 |     295.5 ns |   1.3789 ns |   1.2224 ns |     295.1 ns |  1.00 |    0.00 | 0.1683 |     - |     - |     528 B |
    | CisternLinq |     FSharpList |             1 |     766.5 ns |   4.0689 ns |   3.8061 ns |     766.2 ns |  2.59 |    0.02 | 0.2346 |     - |     - |     736 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |            10 |     860.5 ns |   5.2458 ns |   4.9069 ns |     860.9 ns |  0.66 |    0.00 | 0.2441 |     - |     - |     768 B |
    |  SystemLinq |     FSharpList |            10 |   1,308.8 ns |   7.7574 ns |   7.2563 ns |   1,309.6 ns |  1.00 |    0.00 | 0.4215 |     - |     - |    1328 B |
    | CisternLinq |     FSharpList |            10 |   1,853.0 ns |  12.2432 ns |  11.4523 ns |   1,851.8 ns |  1.42 |    0.01 | 0.4559 |     - |     - |    1432 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |           100 |   5,544.5 ns |  48.9999 ns |  45.8345 ns |   5,535.6 ns |  0.72 |    0.01 | 0.4959 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |           100 |   7,662.1 ns |  55.0558 ns |  51.4993 ns |   7,656.1 ns |  1.00 |    0.00 | 1.6479 |     - |     - |    5200 B |
    | CisternLinq |     FSharpList |           100 |   8,098.2 ns |  48.1450 ns |  45.0349 ns |   8,101.8 ns |  1.06 |    0.01 | 1.1139 |     - |     - |    3504 B |
    |             |                |               |              |             |             |              |       |         |        |       |       |           |
    |     ForLoop |     FSharpList |          1000 |  47,994.3 ns | 202.5132 ns | 189.4309 ns |  48,015.4 ns |  0.85 |    0.00 | 0.4272 |     - |     - |    1576 B |
    |  SystemLinq |     FSharpList |          1000 |  56,592.2 ns | 302.8672 ns | 283.3022 ns |  56,582.0 ns |  1.00 |    0.00 | 8.8501 |     - |     - |   27808 B |
    | CisternLinq |     FSharpList |          1000 |  42,650.5 ns | 278.0330 ns | 260.0722 ns |  42,728.9 ns |  0.75 |    0.00 | 4.6997 |     - |     - |   14801 B |
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
