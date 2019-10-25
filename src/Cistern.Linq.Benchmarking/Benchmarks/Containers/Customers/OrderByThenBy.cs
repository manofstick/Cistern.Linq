using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<Cistern.Linq.Benchmarking.Data.DummyData.Customer>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |         Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |-------------:|--------------:|--------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |          Array |             0 |     137.9 ns |      2.154 ns |      2.015 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |          Array |             0 |     189.7 ns |      2.503 ns |      2.341 ns |  1.38 |    0.03 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |             1 |     264.1 ns |      5.056 ns |      4.729 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |          Array |             1 |     425.4 ns |      4.881 ns |      4.565 ns |  1.61 |    0.04 |  0.1478 |     - |     - |     464 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |            10 |   1,272.9 ns |      6.766 ns |      6.329 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |          Array |            10 |   2,410.0 ns |     19.507 ns |     17.292 ns |  1.89 |    0.02 |  0.2441 |     - |     - |     784 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |           100 |  48,279.2 ns |    411.293 ns |    384.724 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |          Array |           100 |  55,230.3 ns |    548.462 ns |    513.031 ns |  1.14 |    0.02 |  1.2817 |     - |     - |    4024 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |          1000 | 909,216.4 ns |  8,938.328 ns |  8,360.918 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |          Array |          1000 | 880,988.3 ns | 11,161.558 ns | 10,440.528 ns |  0.97 |    0.01 | 10.7422 |     - |     - |   36424 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |             0 |     121.5 ns |      2.459 ns |      2.301 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |           List |             0 |     176.5 ns |      1.296 ns |      1.149 ns |  1.46 |    0.02 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |             1 |     241.6 ns |      1.839 ns |      1.720 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |           List |             1 |     408.6 ns |      5.096 ns |      4.767 ns |  1.69 |    0.02 |  0.1478 |     - |     - |     464 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |            10 |   1,246.7 ns |      9.792 ns |      9.160 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |           List |            10 |   2,276.1 ns |     32.084 ns |     30.011 ns |  1.83 |    0.03 |  0.2441 |     - |     - |     784 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |           100 |  49,420.8 ns |    625.939 ns |    585.504 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |           List |           100 |  54,330.5 ns |    704.279 ns |    658.783 ns |  1.10 |    0.02 |  1.2207 |     - |     - |    4024 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |          1000 | 898,155.5 ns |  6,624.897 ns |  5,872.799 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |           List |          1000 | 880,231.1 ns | 11,939.676 ns | 11,168.380 ns |  0.98 |    0.02 | 10.7422 |     - |     - |   36424 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             0 |     136.1 ns |      1.242 ns |      1.162 ns |  1.00 |    0.00 |  0.0660 |     - |     - |     208 B |
    | CisternLinq |     Enumerable |             0 |     317.2 ns |      3.184 ns |      2.979 ns |  2.33 |    0.03 |  0.0887 |     - |     - |     280 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             1 |     264.5 ns |      3.196 ns |      2.990 ns |  1.00 |    0.00 |  0.1884 |     - |     - |     592 B |
    | CisternLinq |     Enumerable |             1 |     596.2 ns |      8.231 ns |      7.699 ns |  2.25 |    0.04 |  0.2012 |     - |     - |     632 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |            10 |   1,441.0 ns |     14.472 ns |     13.537 ns |  1.00 |    0.00 |  0.3624 |     - |     - |    1144 B |
    | CisternLinq |     Enumerable |            10 |   2,785.8 ns |     51.097 ns |     47.796 ns |  1.93 |    0.04 |  0.3548 |     - |     - |    1128 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |           100 |  50,246.1 ns |    599.632 ns |    500.720 ns |  1.00 |    0.00 |  1.7090 |     - |     - |    5528 B |
    | CisternLinq |     Enumerable |           100 |  58,717.3 ns |    414.063 ns |    323.273 ns |  1.17 |    0.01 |  1.6479 |     - |     - |    5336 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |          1000 | 927,545.9 ns |  4,220.787 ns |  3,741.618 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45136 B |
    | CisternLinq |     Enumerable |          1000 | 933,513.0 ns |  9,565.214 ns |  8,947.307 ns |  1.01 |    0.01 | 13.6719 |     - |     - |   45064 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             0 |     131.6 ns |      2.709 ns |      4.744 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq | ImmutableArray |             0 |     180.9 ns |      3.650 ns |      4.873 ns |  1.36 |    0.08 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             1 |     253.8 ns |      4.151 ns |      3.883 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq | ImmutableArray |             1 |     422.0 ns |      5.750 ns |      5.379 ns |  1.66 |    0.04 |  0.1473 |     - |     - |     464 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |            10 |   1,242.2 ns |     13.685 ns |     12.801 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq | ImmutableArray |            10 |   2,394.3 ns |     31.480 ns |     29.446 ns |  1.93 |    0.03 |  0.2480 |     - |     - |     784 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |           100 |  48,910.1 ns |    446.138 ns |    417.318 ns |  1.00 |    0.00 |  1.2207 |     - |     - |    4128 B |
    | CisternLinq | ImmutableArray |           100 |  53,854.2 ns |    801.940 ns |    750.136 ns |  1.10 |    0.02 |  1.2817 |     - |     - |    4024 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |          1000 | 860,965.9 ns | 12,627.308 ns | 11,811.591 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq | ImmutableArray |          1000 | 897,964.8 ns | 10,784.136 ns | 10,087.487 ns |  1.04 |    0.02 | 10.7422 |     - |     - |   36424 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             0 |     116.4 ns |      1.917 ns |      1.793 ns |  1.00 |    0.00 |  0.0458 |     - |     - |     144 B |
    | CisternLinq |  ImmutableList |             0 |     170.2 ns |      2.018 ns |      1.887 ns |  1.46 |    0.03 |  0.0534 |     - |     - |     168 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             1 |     578.5 ns |      8.249 ns |      7.716 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |  ImmutableList |             1 |     760.6 ns |     10.427 ns |      9.754 ns |  1.32 |    0.03 |  0.1478 |     - |     - |     464 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |            10 |   2,324.1 ns |     22.753 ns |     21.283 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |  ImmutableList |            10 |   3,531.7 ns |     37.462 ns |     35.042 ns |  1.52 |    0.02 |  0.2480 |     - |     - |     784 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |           100 |  57,639.3 ns |    690.013 ns |    645.439 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |  ImmutableList |           100 |  63,161.6 ns |    663.904 ns |    621.016 ns |  1.10 |    0.02 |  1.2207 |     - |     - |    4024 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |          1000 | 930,632.6 ns | 11,867.290 ns | 11,100.670 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |  ImmutableList |          1000 | 946,475.0 ns | 15,655.081 ns | 14,643.773 ns |  1.02 |    0.02 | 10.7422 |     - |     - |   36424 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             0 |     134.1 ns |      2.132 ns |      1.890 ns |  1.00 |    0.00 |  0.0587 |     - |     - |     184 B |
    | CisternLinq |     FSharpList |             0 |     357.0 ns |      3.851 ns |      3.602 ns |  2.66 |    0.06 |  0.0687 |     - |     - |     216 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             1 |     252.9 ns |      2.600 ns |      2.305 ns |  1.00 |    0.00 |  0.1807 |     - |     - |     568 B |
    | CisternLinq |     FSharpList |             1 |     611.4 ns |      5.057 ns |      4.731 ns |  2.42 |    0.02 |  0.1802 |     - |     - |     568 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |            10 |   1,396.2 ns |     20.728 ns |     19.389 ns |  1.00 |    0.00 |  0.3567 |     - |     - |    1120 B |
    | CisternLinq |     FSharpList |            10 |   2,851.3 ns |     40.625 ns |     38.001 ns |  2.04 |    0.04 |  0.3357 |     - |     - |    1064 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |           100 |  50,651.7 ns |    742.053 ns |    694.116 ns |  1.00 |    0.00 |  1.7090 |     - |     - |    5504 B |
    | CisternLinq |     FSharpList |           100 |  56,082.3 ns |    431.205 ns |    403.349 ns |  1.11 |    0.02 |  1.6479 |     - |     - |    5272 B |
    |             |                |               |              |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |          1000 | 900,123.7 ns |  6,368.835 ns |  5,318.265 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45112 B |
    | CisternLinq |     FSharpList |          1000 | 915,661.6 ns | 10,519.394 ns |  9,839.848 ns |  1.02 |    0.01 | 13.6719 |     - |     - |   45004 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_OrderByThenBy : CustomersBase
    {
        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.ThenBy(
                        System.Linq.Enumerable.OrderBy(
                            Customers,
                            c => c.State),
                        c => c.Name)
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.State)
                .ThenBy(c => c.Name)
                .ToList();
        }

    }
}
