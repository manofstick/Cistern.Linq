using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<Cistern.Linq.Benchmarking.Data.DummyData.Customer>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |            Mean |          Error |         StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |----------------:|---------------:|---------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |          Array |             0 |        97.70 ns |      1.6842 ns |      1.4930 ns |  1.00 |    0.00 |  0.0279 |     - |     - |      88 B |
    | CisternLinq |          Array |             0 |       148.55 ns |      2.6139 ns |      2.4450 ns |  1.52 |    0.04 |  0.0355 |     - |     - |     112 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |             1 |       193.06 ns |      2.7045 ns |      2.5298 ns |  1.00 |    0.00 |  0.1147 |     - |     - |     360 B |
    | CisternLinq |          Array |             1 |       352.73 ns |      5.9246 ns |      5.2520 ns |  1.83 |    0.04 |  0.1044 |     - |     - |     328 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |            10 |     2,280.79 ns |     27.5514 ns |     25.7716 ns |  1.00 |    0.00 |  0.2136 |     - |     - |     672 B |
    | CisternLinq |          Array |            10 |     2,943.09 ns |     33.2559 ns |     31.1076 ns |  1.29 |    0.02 |  0.1831 |     - |     - |     576 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |           100 |    51,004.60 ns |    332.3915 ns |    294.6564 ns |  1.00 |    0.00 |  0.9766 |     - |     - |    3192 B |
    | CisternLinq |          Array |           100 |    57,729.58 ns |    555.6026 ns |    519.7110 ns |  1.13 |    0.01 |  0.9766 |     - |     - |    3096 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |          1000 |   894,434.75 ns |  6,443.8614 ns |  6,027.5919 ns |  1.00 |    0.00 |  8.7891 |     - |     - |   28392 B |
    | CisternLinq |          Array |          1000 |   882,946.78 ns |  9,842.9800 ns |  9,207.1296 ns |  0.99 |    0.01 |  8.7891 |     - |     - |   28296 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |             0 |        78.34 ns |      0.7952 ns |      0.7438 ns |  1.00 |    0.00 |  0.0279 |     - |     - |      88 B |
    | CisternLinq |           List |             0 |       131.23 ns |      1.6901 ns |      1.5809 ns |  1.68 |    0.03 |  0.0355 |     - |     - |     112 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |             1 |       168.06 ns |      2.4178 ns |      2.1433 ns |  1.00 |    0.00 |  0.1147 |     - |     - |     360 B |
    | CisternLinq |           List |             1 |       336.74 ns |      4.2143 ns |      3.9421 ns |  2.01 |    0.04 |  0.1044 |     - |     - |     328 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |            10 |     2,043.83 ns |     28.9471 ns |     27.0771 ns |  1.00 |    0.00 |  0.2136 |     - |     - |     672 B |
    | CisternLinq |           List |            10 |     2,952.37 ns |     42.9537 ns |     40.1789 ns |  1.44 |    0.02 |  0.1793 |     - |     - |     576 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |           100 |    50,001.46 ns |    361.0641 ns |    320.0739 ns |  1.00 |    0.00 |  0.9766 |     - |     - |    3192 B |
    | CisternLinq |           List |           100 |    55,102.25 ns |    663.1301 ns |    620.2923 ns |  1.10 |    0.01 |  0.9766 |     - |     - |    3096 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |          1000 |   976,202.34 ns |  3,727.5183 ns |  3,486.7230 ns |  1.00 |    0.00 |  7.8125 |     - |     - |   28392 B |
    | CisternLinq |           List |          1000 |   972,651.36 ns | 13,580.9205 ns | 12,703.6014 ns |  1.00 |    0.01 |  7.8125 |     - |     - |   28296 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             0 |        92.75 ns |      1.9161 ns |      1.8819 ns |  1.00 |    0.00 |  0.0483 |     - |     - |     152 B |
    | CisternLinq |     Enumerable |             0 |       270.97 ns |      3.9277 ns |      3.6740 ns |  2.93 |    0.08 |  0.0710 |     - |     - |     224 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             1 |       191.34 ns |      2.3210 ns |      2.1711 ns |  1.00 |    0.00 |  0.1428 |     - |     - |     448 B |
    | CisternLinq |     Enumerable |             1 |       494.93 ns |      8.0122 ns |      7.4947 ns |  2.59 |    0.05 |  0.1574 |     - |     - |     496 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |            10 |     2,231.91 ns |     27.9429 ns |     26.1378 ns |  1.00 |    0.00 |  0.2899 |     - |     - |     928 B |
    | CisternLinq |     Enumerable |            10 |     3,255.43 ns |     40.7488 ns |     38.1165 ns |  1.46 |    0.02 |  0.2899 |     - |     - |     920 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |           100 |    58,172.98 ns |    513.5653 ns |    455.2622 ns |  1.00 |    0.00 |  1.4038 |     - |     - |    4592 B |
    | CisternLinq |     Enumerable |           100 |    56,891.47 ns |    635.5780 ns |    563.4233 ns |  0.98 |    0.01 |  1.4038 |     - |     - |    4408 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |          1000 |   956,129.90 ns |  7,436.0965 ns |  6,591.9059 ns |  1.00 |    0.00 | 11.7188 |     - |     - |   37000 B |
    | CisternLinq |     Enumerable |          1000 |   943,242.27 ns | 12,964.3375 ns | 12,126.8493 ns |  0.99 |    0.01 | 11.7188 |     - |     - |   36936 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             0 |        83.41 ns |      1.2526 ns |      1.1716 ns |  1.00 |    0.00 |  0.0280 |     - |     - |      88 B |
    | CisternLinq | ImmutableArray |             0 |       128.75 ns |      1.8432 ns |      1.7242 ns |  1.54 |    0.03 |  0.0355 |     - |     - |     112 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             1 |       176.38 ns |      1.7363 ns |      1.6241 ns |  1.00 |    0.00 |  0.1144 |     - |     - |     360 B |
    | CisternLinq | ImmutableArray |             1 |       349.60 ns |      3.6609 ns |      3.4244 ns |  1.98 |    0.02 |  0.1044 |     - |     - |     328 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |            10 |     2,008.30 ns |     31.4284 ns |     29.3981 ns |  1.00 |    0.00 |  0.2098 |     - |     - |     672 B |
    | CisternLinq | ImmutableArray |            10 |     2,960.79 ns |     44.3679 ns |     39.3310 ns |  1.47 |    0.03 |  0.1793 |     - |     - |     576 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |           100 |    51,080.83 ns |    746.7814 ns |    698.5398 ns |  1.00 |    0.00 |  0.9766 |     - |     - |    3192 B |
    | CisternLinq | ImmutableArray |           100 |    58,403.46 ns |    855.8409 ns |    800.5541 ns |  1.14 |    0.02 |  0.9766 |     - |     - |    3096 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |          1000 |   939,629.27 ns | 16,517.1166 ns | 15,450.1211 ns |  1.00 |    0.00 |  8.7891 |     - |     - |   28392 B |
    | CisternLinq | ImmutableArray |          1000 |   880,093.51 ns | 11,560.2573 ns | 10,813.4718 ns |  0.94 |    0.02 |  8.7891 |     - |     - |   28296 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             0 |        77.41 ns |      1.3169 ns |      1.0997 ns |  1.00 |    0.00 |  0.0279 |     - |     - |      88 B |
    | CisternLinq |  ImmutableList |             0 |       129.09 ns |      2.6125 ns |      2.5658 ns |  1.67 |    0.04 |  0.0355 |     - |     - |     112 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             1 |       493.60 ns |      6.3932 ns |      5.9802 ns |  1.00 |    0.00 |  0.1135 |     - |     - |     360 B |
    | CisternLinq |  ImmutableList |             1 |       671.71 ns |      8.5529 ns |      8.0003 ns |  1.36 |    0.02 |  0.1040 |     - |     - |     328 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |            10 |     3,002.82 ns |     35.3205 ns |     33.0388 ns |  1.00 |    0.00 |  0.2136 |     - |     - |     672 B |
    | CisternLinq |  ImmutableList |            10 |     3,855.80 ns |     55.7416 ns |     52.1407 ns |  1.28 |    0.02 |  0.1755 |     - |     - |     576 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |           100 |    63,126.88 ns |    971.8568 ns |    909.0755 ns |  1.00 |    0.00 |  0.9766 |     - |     - |    3192 B |
    | CisternLinq |  ImmutableList |           100 |    65,794.70 ns |    790.1844 ns |    739.1390 ns |  1.04 |    0.02 |  0.9766 |     - |     - |    3096 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |          1000 | 1,046,605.39 ns | 14,733.4559 ns | 13,781.6838 ns |  1.00 |    0.00 |  7.8125 |     - |     - |   28392 B |
    | CisternLinq |  ImmutableList |          1000 |   970,103.91 ns | 15,930.9244 ns | 14,901.7966 ns |  0.93 |    0.02 |  8.7891 |     - |     - |   28296 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             0 |        88.92 ns |      1.0042 ns |      0.9393 ns |  1.00 |    0.00 |  0.0408 |     - |     - |     128 B |
    | CisternLinq |     FSharpList |             0 |       299.12 ns |      4.0340 ns |      3.7734 ns |  3.36 |    0.07 |  0.0505 |     - |     - |     160 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             1 |       181.71 ns |      2.2934 ns |      2.1452 ns |  1.00 |    0.00 |  0.1349 |     - |     - |     424 B |
    | CisternLinq |     FSharpList |             1 |       549.72 ns |      6.9328 ns |      6.4849 ns |  3.03 |    0.04 |  0.1373 |     - |     - |     432 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |            10 |     2,321.34 ns |     35.1065 ns |     32.8387 ns |  1.00 |    0.00 |  0.2861 |     - |     - |     904 B |
    | CisternLinq |     FSharpList |            10 |     3,395.25 ns |     55.8452 ns |     52.2376 ns |  1.46 |    0.03 |  0.2708 |     - |     - |     856 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |           100 |    52,706.27 ns |    534.0642 ns |    499.5640 ns |  1.00 |    0.00 |  1.4038 |     - |     - |    4568 B |
    | CisternLinq |     FSharpList |           100 |    60,898.36 ns |  1,003.6793 ns |    938.8423 ns |  1.16 |    0.02 |  1.3428 |     - |     - |    4344 B |
    |             |                |               |                 |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |          1000 |   984,875.33 ns | 17,536.8291 ns | 16,403.9608 ns |  1.00 |    0.00 | 11.7188 |     - |     - |   36976 B |
    | CisternLinq |     FSharpList |          1000 |   918,030.45 ns | 14,777.4542 ns | 13,822.8398 ns |  0.93 |    0.01 | 11.7188 |     - |     - |   36876 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_OrderBy : CustomersBase
    {
        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.OrderBy(
                        Customers,
                        c => c.Name
                    )
                );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.Name)
                .ToList();
        }

    }
}
